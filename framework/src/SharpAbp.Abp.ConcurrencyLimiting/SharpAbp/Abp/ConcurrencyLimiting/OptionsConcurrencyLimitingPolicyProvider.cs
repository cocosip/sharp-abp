using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.ConcurrencyLimiting
{
    public class OptionsConcurrencyLimitingPolicyProvider : IConcurrencyLimitingPolicyProvider, ISingletonDependency
    {
        private readonly Dictionary<string, ConcurrencyLimitingPolicy> _policies;
        public OptionsConcurrencyLimitingPolicyProvider(IOptions<SharpAbpConcurrencyLimitingOptions> options)
        {
            _policies = new Dictionary<string, ConcurrencyLimitingPolicy>(options.Value.Policies, StringComparer.Ordinal);
            foreach (var policy in _policies.Values)
                if (policy.PartitionKind == ConcurrencyLimitingPartitionKind.Custom && !options.Value.Resolvers.ContainsKey(policy.ResolverName!))
                {
                    throw new ConcurrencyLimitingConfigurationException("Unknown concurrency resolver: " + policy.ResolverName);
                }
        }

        public virtual ConcurrencyLimitingPolicy Get(string name)
        {
            SharpAbpConcurrencyLimitingOptions.ValidateName(name, nameof(name));
            if (!_policies.TryGetValue(name, out var policy))
            {
                throw new ConcurrencyLimitingConfigurationException("Unknown concurrency policy: " + name);
            }
            return policy;
        }
    }

}
