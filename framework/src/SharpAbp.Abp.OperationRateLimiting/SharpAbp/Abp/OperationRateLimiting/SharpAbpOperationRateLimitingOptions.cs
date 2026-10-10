using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public class SharpAbpOperationRateLimitingOptions
    {
        public bool IsEnabled { get; set; } = true;
        public string ApplicationName { get; set; } = "sharp-abp";
        public TimeSpan LockTimeout { get; set; } = TimeSpan.FromSeconds(5);
        public OperationRateLimitingCoordinationScope CoordinationScope { get; set; }

        public IDictionary<string, OperationRateLimitingPolicy> Policies { get; } =
            new Dictionary<string, OperationRateLimitingPolicy>(StringComparer.Ordinal);

        public IDictionary<string, Func<IServiceProvider, OperationRateLimitingContext, CancellationToken, Task<string>>> PartitionResolvers { get; } =
            new Dictionary<string, Func<IServiceProvider, OperationRateLimitingContext, CancellationToken, Task<string>>>(StringComparer.Ordinal);

        public virtual SharpAbpOperationRateLimitingOptions AddPolicy(string name, Action<OperationRateLimitingPolicyBuilder> configure)
        {
            ValidateName(name, nameof(name));
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }
            var builder = new OperationRateLimitingPolicyBuilder();
            configure(builder);
            Policies[name] = builder.Build(name);
            return this;
        }

        public virtual SharpAbpOperationRateLimitingOptions ConfigurePolicy(string name, Action<OperationRateLimitingPolicyBuilder> configure)
        {
            ValidateName(name, nameof(name));
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }
            if (!Policies.TryGetValue(name, out var policy))
            {
                throw new OperationRateLimitingConfigurationException("Unknown policy: " + name);
            }
            var builder = new OperationRateLimitingPolicyBuilder(policy);
            configure(builder);
            Policies[name] = builder.Build(name);
            return this;
        }

        public virtual SharpAbpOperationRateLimitingOptions RemovePolicy(string name)
        {
            ValidateName(name, nameof(name));
            Policies.Remove(name);
            return this;
        }

        public virtual SharpAbpOperationRateLimitingOptions AddPartitionResolver(string name,
            Func<IServiceProvider, OperationRateLimitingContext, CancellationToken, Task<string>> resolver)
        {
            if (string.IsNullOrWhiteSpace(name) || OperationRateLimitingPartitionResolver.IsBuiltIn(name))
            {
                throw new OperationRateLimitingConfigurationException("Custom resolver name must be nonempty and not a built-in name.");
            }
            PartitionResolvers[name] = resolver ?? throw new ArgumentNullException(nameof(resolver));
            return this;
        }

        internal static void ValidateName(string name, string parameter)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("A nonempty name is required.", parameter);
            }
        }
    }
}
