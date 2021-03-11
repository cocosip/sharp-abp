using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.MapTenancy
{
    [DependsOn(
        typeof(AbpMultiTenancyModule)
        )]
    public class AbpMapTenancyModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpMapTenancyOptions>(options => { });
        }
    }
}
