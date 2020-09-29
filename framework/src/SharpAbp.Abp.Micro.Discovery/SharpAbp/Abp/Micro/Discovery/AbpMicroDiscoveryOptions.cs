namespace SharpAbp.Abp.Micro.Discovery
{
    public class AbpMicroDiscoveryOptions
    {
        public ServiceDiscoveryConfigurations Configurations { get; }

        public AbpMicroDiscoveryOptions()
        {
            Configurations = new ServiceDiscoveryConfigurations();
        }

    }

}
