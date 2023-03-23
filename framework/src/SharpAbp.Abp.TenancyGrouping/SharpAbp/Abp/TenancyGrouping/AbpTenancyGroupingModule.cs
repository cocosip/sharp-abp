using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.TenancyGrouping
{
    [DependsOn(
        typeof(AbpMultiTenancyModule)
        )]
    public class AbpTenancyGroupingModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpTenancyGroupingOptions>(options =>
            {
                options.IsEnabled = true;
            });
        }
    }
}
