using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Micro.Discovery.AddressTable
{
    [DependsOn(
        typeof(AbpMicroDiscoveryModule)
    )]
    public class AbpMicroDiscoveryAddressTableModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpMicroDiscoveryAddressTableOptions>(c =>
            {

            });
        }
    }
}
