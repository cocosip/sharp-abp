using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Micro.Discovery.Consul
{
    [DependsOn(
        typeof(AbpMicroDiscoveryModule)
        )]
    public class AbpMicroDiscoveryConsulModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpMicroDiscoveryConsulOptions>(options => { });
        }

    }
}
