using SharpAbp.Abp.Micro.Discovery;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Micro.ServiceDiscovery
{
    [DependsOn(
        typeof(AbpMicroModule)
    )]
    public class AbpMicroDiscoveryModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpMicroDiscoveryOptions>(c =>
            {
                c.DiscoveryServices.ConfigureDefault(d =>
                {
                    
                });
            });
        }
    }
}
