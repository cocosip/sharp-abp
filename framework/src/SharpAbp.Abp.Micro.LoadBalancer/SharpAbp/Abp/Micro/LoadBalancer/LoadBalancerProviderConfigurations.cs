using JetBrains.Annotations;
using System.Collections.Generic;
using Volo.Abp;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public class LoadBalancerProviderConfigurations
    {
        private readonly Dictionary<string, LoadBalancerProviderConfiguration> _providers;

        public LoadBalancerProviderConfigurations()
        {
            _providers = new Dictionary<string, LoadBalancerProviderConfiguration>();
        }


        public LoadBalancerProviderConfiguration GetConfiguration([NotNull] string balanceType)
        {
            Check.NotNullOrWhiteSpace(balanceType, nameof(balanceType));
            return _providers.GetOrDefault(balanceType);
        }

        public bool TryAdd([NotNull] LoadBalancerProviderConfiguration configuration)
        {
            Check.NotNull(configuration, nameof(configuration));

            if (_providers.ContainsKey(configuration.BalancerType))
            {
                return false;
            }
            _providers.Add(configuration.BalancerType, configuration);
            return true;
        }

        public bool TryRemove([NotNull] string balanceType)
        {
            Check.NotNull(balanceType, nameof(balanceType));
            return _providers.Remove(balanceType);
        }

    }
}
