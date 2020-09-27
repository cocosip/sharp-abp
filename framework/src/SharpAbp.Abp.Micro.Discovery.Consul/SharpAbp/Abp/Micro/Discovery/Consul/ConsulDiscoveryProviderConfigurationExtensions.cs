using System;

namespace SharpAbp.Abp.Micro.Discovery.Consul
{
    public static class ConsulDiscoveryProviderConfigurationExtensions
    {

        public static ConsulDiscoveryProviderConfiguration GetConsulDiscoveryConfiguration(
         this DiscoveryConfiguration discoveryConfiguration)
        {
            return new ConsulDiscoveryProviderConfiguration(discoveryConfiguration);
        }

        public static DiscoveryConfiguration UseConsulDiscovery(
            this DiscoveryConfiguration containerConfiguration,
            Action<ConsulDiscoveryProviderConfiguration> consulConfigureAction)
        {
            containerConfiguration.ProviderType = typeof(ConsulServiceDiscoveryProvider);

            consulConfigureAction(new ConsulDiscoveryProviderConfiguration(containerConfiguration));

            return containerConfiguration;
        }
    }
}
