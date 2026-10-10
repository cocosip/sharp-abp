using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.ConcurrencyLimiting
{
    public class SharpAbpConcurrencyLimitingOptions
    {
        public bool IsEnabled { get; set; } = true;
        internal Dictionary<string, ConcurrencyLimitingPolicy> Policies { get; } = new Dictionary<string, ConcurrencyLimitingPolicy>(StringComparer.Ordinal);
        internal Dictionary<string, Func<IServiceProvider, ConcurrencyLimitingContext, string>> Resolvers { get; } = new Dictionary<string, Func<IServiceProvider, ConcurrencyLimitingContext, string>>(StringComparer.Ordinal);
        public SharpAbpConcurrencyLimitingOptions AddPolicy(string name, Action<ConcurrencyLimitingPolicyBuilder> configure)
        {
            ValidateName(name, nameof(name));
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }
            var builder = new ConcurrencyLimitingPolicyBuilder();
            configure(builder);
            Policies[name] = builder.Build(name);
            return this;
        }

        public SharpAbpConcurrencyLimitingOptions ConfigurePolicy(string name, Action<ConcurrencyLimitingPolicyBuilder> configure)
        {
            ValidateName(name, nameof(name));
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }
            if (!Policies.TryGetValue(name, out var policy))
            {
                throw new ConcurrencyLimitingConfigurationException("Unknown concurrency policy: " + name);
            }
            var builder = new ConcurrencyLimitingPolicyBuilder(policy);
            configure(builder);
            Policies[name] = builder.Build(name);
            return this;
        }

        public SharpAbpConcurrencyLimitingOptions AddPartitionKeyResolver(string name, Func<IServiceProvider, ConcurrencyLimitingContext, string> resolver)
        {
            ValidateName(name, nameof(name));
            if (resolver == null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }
            Resolvers[name] = resolver;
            return this;
        }

        public SharpAbpConcurrencyLimitingOptions ReplacePartitionKeyResolver(string name, Func<IServiceProvider, ConcurrencyLimitingContext, string> resolver)
        {
            ValidateName(name, nameof(name));
            if (resolver == null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }
            if (!Resolvers.ContainsKey(name))
            {
                throw new ConcurrencyLimitingConfigurationException("Unknown concurrency resolver: " + name);
            }
            Resolvers[name] = resolver;
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
