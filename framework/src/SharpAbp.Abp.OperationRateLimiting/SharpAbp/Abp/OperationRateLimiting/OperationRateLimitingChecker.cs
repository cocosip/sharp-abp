using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public class OperationRateLimitingChecker : IOperationRateLimitingChecker, ITransientDependency
    {
        protected OperationRateLimitingConfiguration Configuration { get; }
        protected IOperationRateLimitingPolicyProvider PolicyProvider { get; }
        protected IOperationRateLimitingPartitionResolver PartitionResolver { get; }
        protected IOperationRateLimitingKeyBuilder KeyBuilder { get; }
        protected IOperationRateLimitingFormatter Formatter { get; }
        protected IServiceProvider ServiceProvider { get; }

        public OperationRateLimitingChecker(OperationRateLimitingConfiguration configuration,
            IOperationRateLimitingPolicyProvider policyProvider, IOperationRateLimitingPartitionResolver partitionResolver,
            IOperationRateLimitingKeyBuilder keyBuilder, IOperationRateLimitingFormatter formatter, IServiceProvider serviceProvider)
        {
            Configuration = configuration;
            PolicyProvider = policyProvider;
            PartitionResolver = partitionResolver;
            KeyBuilder = keyBuilder;
            Formatter = formatter;
            ServiceProvider = serviceProvider;
        }

        protected virtual void ValidateCall(string policyName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(policyName))
            {
                throw new OperationRateLimitingConfigurationException("Policy name must be nonempty.");
            }
            cancellationToken.ThrowIfCancellationRequested();
        }

        protected virtual async Task<IReadOnlyList<OperationRateLimitingRuleContext>> ResolveAsync(
            OperationRateLimitingPolicy policy, OperationRateLimitingContext? context, CancellationToken cancellationToken)
        {
            var businessContext = context ?? new OperationRateLimitingContext();
            var rules = new List<OperationRateLimitingRuleContext>();
            foreach (var definition in policy.Rules)
            {
                IReadOnlyList<string> partitionValues;
                if (definition.Partitions.Count == 1)
                {
                    partitionValues = new[]
                    {
                        await PartitionResolver.ResolveAsync(definition, businessContext, cancellationToken)
                            .ConfigureAwait(false)
                    };
                }
                else if (PartitionResolver is IOperationRateLimitingCompositePartitionResolver compositeResolver)
                {
                    partitionValues = await compositeResolver.ResolvePartitionsAsync(
                        definition, businessContext, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    throw new OperationRateLimitingConfigurationException(
                        "Composite partitions require an IOperationRateLimitingCompositePartitionResolver implementation.");
                }

                string partition;
                if (definition.Partitions.Count == 1)
                {
                    partition = partitionValues[0];
                }
                else
                {
                    var partitionComponents = new List<string>(partitionValues.Count * 2);
                    for (var index = 0; index < partitionValues.Count; index++)
                    {
                        partitionComponents.Add(definition.Partitions[index].Name);
                        partitionComponents.Add(partitionValues[index]);
                    }

                    partition = OperationRateLimitingKeyBuilder.HashComponents(partitionComponents.ToArray());
                }
                rules.Add(new OperationRateLimitingRuleContext(definition, businessContext,
                    KeyBuilder.Build(Configuration.ApplicationName, policy.Name, definition, partition, PartitionResolver.GetTenantKey())));
            }
            return rules;
        }

        protected virtual IOperationRateLimitingRule ResolveRule(OperationRateLimitingRuleContext context)
        {
            var rule = ServiceProvider.GetService(context.Definition.RuleType) as IOperationRateLimitingRule;
            if (rule == null)
            {
                throw new OperationRateLimitingConfigurationException("Rule is not registered: " + context.Definition.RuleType.FullName);
            }
            return rule;
        }

        public virtual async Task CheckAsync(string policyName, OperationRateLimitingContext? context = null,
            CancellationToken cancellationToken = default)
        {
            ValidateCall(policyName, cancellationToken);
            if (!Configuration.IsEnabled)
            {
                return;
            }
            var policy = await PolicyProvider.GetAsync(policyName, cancellationToken).ConfigureAwait(false);
            var contexts = await ResolveAsync(policy, context, cancellationToken).ConfigureAwait(false);
            var rules = contexts.Select(ResolveRule).ToArray();
            if (contexts.Count == 1)
            {
                var result = await rules[0].ConsumeAsync(contexts[0], cancellationToken).ConfigureAwait(false);
                if (!result.IsAccepted)
                {
                    throw CreateExceededException(policy, result.Status);
                }
                return;
            }
            var statuses = new List<OperationRateLimitingRuleStatus>();
            if (rules.All(rule => rule.GetType() == typeof(FixedWindowOperationRateLimitingRule)))
            {
                statuses.AddRange(await Task.WhenAll(contexts.Select((context, index) =>
                    rules[index].GetStatusAsync(context, cancellationToken))).ConfigureAwait(false));
            }
            else
            {
                for (var index = 0; index < contexts.Count; index++)
                {
                    statuses.Add(await rules[index].GetStatusAsync(contexts[index], cancellationToken).ConfigureAwait(false));
                }
            }
            var blocked = statuses.Where(status => !status.IsAllowed).ToArray();
            if (blocked.Length > 0)
            {
                var limiting = blocked.FirstOrDefault(status => !status.RetryAfter.HasValue) ??
                    blocked.OrderByDescending(status => status.RetryAfter).First();
                throw CreateExceededException(policy, limiting);
            }
            for (var index = 0; index < contexts.Count; index++)
            {
                var result = await rules[index].ConsumeAsync(contexts[index], cancellationToken).ConfigureAwait(false);
                if (!result.IsAccepted)
                {
                    throw CreateExceededException(policy, result.Status);
                }
            }
        }

        protected virtual OperationRateLimitExceededException CreateExceededException(OperationRateLimitingPolicy policy,
            OperationRateLimitingRuleStatus status)
        {
            return new OperationRateLimitExceededException(policy.Name, status, policy.ErrorCode,
                status.RetryAfter.HasValue ? Formatter.Format(status.RetryAfter.Value) : null);
        }

        public virtual async Task<bool> IsAllowedAsync(string policyName, OperationRateLimitingContext? context = null,
            CancellationToken cancellationToken = default)
        {
            return (await GetStatusAsync(policyName, context, cancellationToken).ConfigureAwait(false)).IsAllowed;
        }

        public virtual async Task<OperationRateLimitingStatus> GetStatusAsync(string policyName,
            OperationRateLimitingContext? context = null, CancellationToken cancellationToken = default)
        {
            ValidateCall(policyName, cancellationToken);
            if (!Configuration.IsEnabled)
            {
                return new OperationRateLimitingStatus(false, true, int.MaxValue, 0, int.MaxValue, null,
                    Array.Empty<OperationRateLimitingRuleStatus>());
            }
            var policy = await PolicyProvider.GetAsync(policyName, cancellationToken).ConfigureAwait(false);
            var contexts = await ResolveAsync(policy, context, cancellationToken).ConfigureAwait(false);
            var statuses = new List<OperationRateLimitingRuleStatus>();
            foreach (var ruleContext in contexts)
            {
                statuses.Add(await ResolveRule(ruleContext).GetStatusAsync(ruleContext, cancellationToken).ConfigureAwait(false));
            }
            return Aggregate(statuses);
        }

        protected virtual OperationRateLimitingStatus Aggregate(IReadOnlyList<OperationRateLimitingRuleStatus> statuses)
        {
            var limiting = statuses.OrderBy(status => status.RemainingCount).First();
            var blocked = statuses.Where(status => !status.IsAllowed).ToArray();
            TimeSpan? retryAfter = null;
            if (blocked.Length > 0 && blocked.All(status => status.RetryAfter.HasValue))
            {
                retryAfter = blocked.Max(status => status.RetryAfter!.Value);
            }
            return new OperationRateLimitingStatus(true, blocked.Length == 0, limiting.MaxCount,
                limiting.CurrentCount, limiting.RemainingCount, retryAfter, statuses);
        }

        public virtual async Task ResetAsync(string policyName, OperationRateLimitingContext? context = null,
            CancellationToken cancellationToken = default)
        {
            ValidateCall(policyName, cancellationToken);
            if (!Configuration.IsEnabled)
            {
                return;
            }
            var policy = await PolicyProvider.GetAsync(policyName, cancellationToken).ConfigureAwait(false);
            var contexts = await ResolveAsync(policy, context, cancellationToken).ConfigureAwait(false);
            foreach (var ruleContext in contexts)
            {
                await ResolveRule(ruleContext).ResetAsync(ruleContext, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
