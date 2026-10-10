using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Users;

namespace SharpAbp.Abp.OperationRateLimiting
{
    [ExposeServices(typeof(IOperationRateLimitingPartitionResolver),
        typeof(IOperationRateLimitingCompositePartitionResolver))]
    public class OperationRateLimitingPartitionResolver : IOperationRateLimitingPartitionResolver,
        IOperationRateLimitingCompositePartitionResolver, ITransientDependency
    {
        protected ICurrentUser CurrentUser { get; }
        protected ICurrentTenant CurrentTenant { get; }
        protected IServiceProvider ServiceProvider { get; }
        protected OperationRateLimitingConfiguration Configuration { get; }

        public OperationRateLimitingPartitionResolver(ICurrentUser currentUser, ICurrentTenant currentTenant,
            IServiceProvider serviceProvider, OperationRateLimitingConfiguration configuration)
        {
            CurrentUser = currentUser;
            CurrentTenant = currentTenant;
            ServiceProvider = serviceProvider;
            Configuration = configuration;
        }

        public static bool IsBuiltIn(string name)
        {
            return name == "Global" || name == "Parameter" || name == "CurrentUser" ||
                name == "CurrentTenant" || name == "Email" || name == "PhoneNumber";
        }

        public virtual string GetTenantKey()
        {
            return CurrentTenant.Id?.ToString("D") ?? "host";
        }

        public virtual async Task<string> ResolveAsync(OperationRateLimitingRuleDefinition rule,
            OperationRateLimitingContext context, CancellationToken cancellationToken = default)
        {
            if (rule.Partitions.Count != 1)
            {
                throw new OperationRateLimitingConfigurationException(
                    "The compatibility partition resolver can resolve only one partition.");
            }

            return await ResolvePartitionAsync(rule.Partitions[0], context, cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task<IReadOnlyList<string>> ResolvePartitionsAsync(OperationRateLimitingRuleDefinition rule,
            OperationRateLimitingContext context, CancellationToken cancellationToken = default)
        {
            var values = new List<string>(rule.Partitions.Count);
            foreach (var partition in rule.Partitions)
            {
                values.Add(await ResolvePartitionAsync(partition, context, cancellationToken).ConfigureAwait(false));
            }

            return values;
        }

        protected virtual async Task<string> ResolvePartitionAsync(OperationRateLimitingPartition partition,
            OperationRateLimitingContext context, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            object? value = null;
            string? key;

            switch (partition.Kind)
            {
                case OperationRateLimitingPartitionKind.Global:
                    key = "global";
                    break;
                case OperationRateLimitingPartitionKind.Parameter:
                    key = context.Parameter;
                    break;
                case OperationRateLimitingPartitionKind.Argument:
                    value = context.Arguments.GetValue(partition.ArgumentName!);
                    key = ConvertValue(value);
                    break;
                case OperationRateLimitingPartitionKind.CurrentUser:
                    key = CurrentUser.Id?.ToString("D");
                    break;
                case OperationRateLimitingPartitionKind.CurrentTenant:
                    key = GetTenantKey();
                    break;
                case OperationRateLimitingPartitionKind.Email:
                    value = GetArgumentOrParameter(context, partition.ArgumentName);
                    key = ConvertValue(value)?.Trim().ToUpperInvariant();
                    break;
                case OperationRateLimitingPartitionKind.PhoneNumber:
                    value = GetArgumentOrParameter(context, partition.ArgumentName);
                    key = NormalizePhone(ConvertValue(value));
                    break;
                case OperationRateLimitingPartitionKind.Resolver:
                    if (!Configuration.PartitionResolvers.TryGetValue(partition.ResolverName!, out var resolver))
                    {
                        throw new OperationRateLimitingConfigurationException(
                            "Unknown partition resolver: " + partition.ResolverName);
                    }

                    key = await resolver(ServiceProvider,
                        context.CreatePartitionContext(partition, GetOptionalArgument(context, partition.ArgumentName)),
                        cancellationToken).ConfigureAwait(false);
                    break;
                default:
                    throw new OperationRateLimitingConfigurationException(
                        "Unknown operation rate limiting partition kind: " + partition.Kind);
            }

            cancellationToken.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new OperationRateLimitingContextException("Missing partition identity for " + partition.Name + ".");
            }

            return key!;
        }

        protected virtual object? GetOptionalArgument(OperationRateLimitingContext context, string? argumentName)
        {
            if (argumentName == null || !context.Arguments.TryGetValue(argumentName, out var value))
            {
                return null;
            }

            return value;
        }

        protected virtual object? GetArgumentOrParameter(OperationRateLimitingContext context, string? argumentName)
        {
            return argumentName == null ? context.Parameter : context.Arguments.GetValue(argumentName);
        }

        protected virtual string? ConvertValue(object? value)
        {
            if (value == null)
            {
                return null;
            }

            if (value is string stringValue)
            {
                return stringValue;
            }

            if (value is IFormattable formattable)
            {
                return formattable.ToString(null, CultureInfo.InvariantCulture);
            }

            return value.ToString() ?? string.Empty;
        }

        protected virtual string? NormalizePhone(string? phone)
        {
            return phone == null ? null : new string(phone
                .Where(character => character >= '0' && character <= '9' || character == '+').ToArray());
        }
    }
}
