namespace SharpAbp.Abp.Micro.Discovery.AddressTable
{
    public static class DiscoveryConfigurationExtensions
    {
        public static DiscoveryConfiguration UseAddressTable(this DiscoveryConfiguration configuration)
        {
            configuration.ProviderType = typeof(AddressTableServiceDiscoveryProvider);
            return configuration;
        }
    }
}
