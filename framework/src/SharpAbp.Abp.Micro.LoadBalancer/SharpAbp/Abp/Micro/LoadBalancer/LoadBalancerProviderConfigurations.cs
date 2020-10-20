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


        public LoadBalancerProviderConfiguration GetConfiguration([NotNull] string type)
        {
            Check.NotNullOrWhiteSpace(type, nameof(type));
            return _providers.GetOrDefault(type);
        }

        public bool TryAdd([NotNull] LoadBalancerProviderConfiguration configuration)
        {
            Check.NotNull(configuration, nameof(configuration));

            if (_providers.ContainsKey(configuration.Type))
            {
                return false;
            }
            _providers.Add(configuration.Type, configuration);
            return true;
        }



        public bool TryRemove([NotNull] string type)
        {
            Check.NotNull(type, nameof(type));
            return _providers.Remove(type);
        }

    }
}
