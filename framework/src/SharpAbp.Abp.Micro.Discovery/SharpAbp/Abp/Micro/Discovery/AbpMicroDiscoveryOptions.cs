using JetBrains.Annotations;
using System;
using Volo.Abp;

namespace SharpAbp.Abp.Micro.Discovery
{
    public class AbpMicroDiscoveryOptions
    {
        public Type DefaultDiscoveryProvider { get; private set; }

        public ServiceDiscoveryConfigurations DiscoveryProviders { get; }

        public AbpMicroDiscoveryOptions()
        {
            DefaultDiscoveryProvider = typeof(ConfigurationServiceDiscoveryProvider);
            DiscoveryProviders = new ServiceDiscoveryConfigurations();
        }

        public AbpMicroDiscoveryOptions ConfigureDefault([NotNull] Type defaultDiscoveryProvider)
        {
            Check.NotNull(defaultDiscoveryProvider, nameof(defaultDiscoveryProvider));
            DefaultDiscoveryProvider = defaultDiscoveryProvider;
            return this;
        }



    }

}
