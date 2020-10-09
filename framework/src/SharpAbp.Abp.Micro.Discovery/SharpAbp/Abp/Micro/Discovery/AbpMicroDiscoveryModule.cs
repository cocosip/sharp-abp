using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Micro.Discovery
{
    [DependsOn(
        typeof(AbpMicroModule)
    )]
    public class AbpMicroDiscoveryModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpMicroDiscoveryOptions>(options => { });
        }
    }
}
