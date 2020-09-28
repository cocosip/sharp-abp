using System;

namespace SharpAbp.Abp.Micro.Discovery.AddressTable
{
    public static class AddressTableDiscoveryProviderConfigurationExtensions
    {
        public static AddressTableDiscoveryProviderConfiguration GetAddressTableDiscoveryConfiguration(
       this DiscoveryConfiguration discoveryConfiguration)
        {
            return new AddressTableDiscoveryProviderConfiguration(discoveryConfiguration);
        }

        public static DiscoveryConfiguration UseAddressTableDiscovery(
            this DiscoveryConfiguration containerConfiguration,
            Action<AddressTableDiscoveryProviderConfiguration> addressTableConfigureAction)
        {
            containerConfiguration.ProviderType = typeof(AddressTableServiceDiscoveryProvider);

            addressTableConfigureAction(new AddressTableDiscoveryProviderConfiguration(containerConfiguration));

            return containerConfiguration;
        }



    }
}
