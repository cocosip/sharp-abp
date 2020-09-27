namespace SharpAbp.Abp.Micro.Discovery
{
    public class AbpMicroDiscoveryOptions
    {
        public DiscoveryConfigurations Discoverers { get; }

        public AbpMicroDiscoveryOptions()
        {
            Discoverers = new DiscoveryConfigurations();
        }

    }

}
