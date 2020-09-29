using System;

namespace SharpAbp.Abp.Micro.Discovery.Consul
{
    public static class ConsulDiscoveryProviderConfigurationExtensions
    {

        public static ConsulDiscoveryProviderConfiguration GetConsulDiscoveryConfiguration(
         this ServiceDiscoveryConfiguration discoveryConfiguration)
        {
            return new ConsulDiscoveryProviderConfiguration(discoveryConfiguration);
        }

        public static ServiceDiscoveryConfiguration UseConsulDiscovery(
            this ServiceDiscoveryConfiguration containerConfiguration,
            Action<ConsulDiscoveryProviderConfiguration> consulConfigureAction)
        {
            //containerConfiguration.ProviderType = typeof(ConsulServiceDiscoveryProvider);

            consulConfigureAction(new ConsulDiscoveryProviderConfiguration(containerConfiguration));

            return containerConfiguration;
        }
    }
}
