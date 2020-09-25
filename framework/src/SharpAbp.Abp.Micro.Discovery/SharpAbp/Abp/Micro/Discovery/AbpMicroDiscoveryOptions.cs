namespace SharpAbp.Abp.Micro.Discovery
{
    public class AbpMicroDiscoveryOptions
    {
        public DiscoveryConfigurations DiscoveryServices { get; }

        public AbpMicroDiscoveryOptions()
        {
            DiscoveryServices = new DiscoveryConfigurations();
        }

    }

}
