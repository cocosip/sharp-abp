using SharpAbp.Abp.Micro.Discovery.AddressTable;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Micro.Discovery
{
    [DependsOn(
        typeof(AbpMicroModule)
    )]
    public class AbpMicroDiscoveryModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpMicroDiscoveryOptions>(c => { });
            Configure<AddressTableDiscoveryOptions>(c => { });

        }
    }
}
