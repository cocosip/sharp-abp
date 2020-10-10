namespace SharpAbp.Abp.Micro.Discovery.Consul
{
    public static class DiscoveryConfigurationExtensions
    {
        public static DiscoveryConfiguration UseConsul(this DiscoveryConfiguration configuration)
        {
            configuration.ProviderType = typeof(ConsulServiceDiscoveryProvider);
            return configuration;
        }
    }
}
