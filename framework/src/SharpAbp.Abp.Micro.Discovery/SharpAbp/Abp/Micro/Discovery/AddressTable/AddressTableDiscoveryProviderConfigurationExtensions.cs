using System;

namespace SharpAbp.Abp.Micro.Discovery.AddressTable
{
    public static class AddressTableDiscoveryProviderConfigurationExtensions
    {
        public static AddressTableDiscoveryProviderConfiguration GetAddressTableDiscoveryConfiguration(
       this ServiceDiscoveryConfiguration discoveryConfiguration)
        {
            return new AddressTableDiscoveryProviderConfiguration(discoveryConfiguration);
        }

        public static ServiceDiscoveryConfiguration UseAddressTableDiscovery(
            this ServiceDiscoveryConfiguration containerConfiguration,
            Action<AddressTableDiscoveryProviderConfiguration> addressTableConfigureAction)
        {
            containerConfiguration.ProviderType = typeof(AddressTableServiceDiscoveryProvider);

            addressTableConfigureAction(new AddressTableDiscoveryProviderConfiguration(containerConfiguration));

            return containerConfiguration;
        }



    }
}
