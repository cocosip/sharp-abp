using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public class OperationRateLimitingPolicyBuilder
    {
        protected IList<OperationRateLimitingRuleBuilder> Rules { get; } = new List<OperationRateLimitingRuleBuilder>();
        protected OperationRateLimitingRuleBuilder? ShortcutRule { get; set; }
        protected string? ErrorCode { get; set; }

        public OperationRateLimitingPolicyBuilder()
        {
        }

        public OperationRateLimitingPolicyBuilder(OperationRateLimitingPolicy policy)
        {
            ErrorCode = policy.ErrorCode;
            foreach (var definition in policy.Rules)
            {
                var builder = new OperationRateLimitingRuleBuilder(definition);
                Rules.Add(builder);
                if (ShortcutRule == null && definition.RuleType == typeof(FixedWindowOperationRateLimitingRule))
                {
                    ShortcutRule = builder;
                }
            }
        }

        protected virtual OperationRateLimitingRuleBuilder GetShortcutRule()
        {
            if (ShortcutRule == null)
            {
                ShortcutRule = new OperationRateLimitingRuleBuilder();
                Rules.Add(ShortcutRule);
            }
            return ShortcutRule;
        }

        public virtual OperationRateLimitingPolicyBuilder WithFixedWindow(TimeSpan duration, int maxCount)
        {
            GetShortcutRule().WithFixedWindow(duration, maxCount);
            return this;
        }

        public virtual OperationRateLimitingPolicyBuilder AddRule(Action<OperationRateLimitingRuleBuilder> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }
            var builder = new OperationRateLimitingRuleBuilder();
            configure(builder);
            Rules.Add(builder);
            return this;
        }

        public virtual OperationRateLimitingPolicyBuilder AddRule<TRule>() where TRule : IOperationRateLimitingRule
        {
            return AddRule(builder => builder.WithRule<TRule>());
        }

        public virtual OperationRateLimitingPolicyBuilder WithErrorCode(string errorCode)
        {
            if (string.IsNullOrWhiteSpace(errorCode))
            {
                throw new OperationRateLimitingConfigurationException("Error code must be nonempty.");
            }
            ErrorCode = errorCode;
            return this;
        }

        public virtual OperationRateLimitingPolicyBuilder ClearRules()
        {
            Rules.Clear();
            ShortcutRule = null;
            return this;
        }

        public virtual OperationRateLimitingPolicyBuilder WithMultiTenancy(bool enabled = true)
        {
            GetShortcutRule().WithMultiTenancy(enabled);
            return this;
        }

        public virtual OperationRateLimitingPolicyBuilder PartitionGlobally()
        {
            return PartitionBy(OperationRateLimitingPartition.Global());
        }

        public virtual OperationRateLimitingPolicyBuilder PartitionByParameter()
        {
            return PartitionBy(OperationRateLimitingPartition.Parameter());
        }

        public virtual OperationRateLimitingPolicyBuilder PartitionByCurrentUser()
        {
            return PartitionBy(OperationRateLimitingPartition.CurrentUser());
        }

        public virtual OperationRateLimitingPolicyBuilder PartitionByCurrentTenant()
        {
            return PartitionBy(OperationRateLimitingPartition.CurrentTenant());
        }

        public virtual OperationRateLimitingPolicyBuilder PartitionByEmail()
        {
            return PartitionBy(OperationRateLimitingPartition.Email());
        }

        public virtual OperationRateLimitingPolicyBuilder PartitionByPhoneNumber()
        {
            return PartitionBy(OperationRateLimitingPartition.PhoneNumber());
        }

        public virtual OperationRateLimitingPolicyBuilder PartitionBy(string resolverName)
        {
            GetShortcutRule().PartitionBy(resolverName);
            return this;
        }

        public virtual OperationRateLimitingPolicyBuilder PartitionBy(
            params OperationRateLimitingPartition[] partitions)
        {
            GetShortcutRule().PartitionBy(partitions);
            return this;
        }

        public virtual OperationRateLimitingPolicy Build(string name)
        {
            return new OperationRateLimitingPolicy(name, Rules.Select(builder => builder.Build()), ErrorCode);
        }
    }
}
