using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.TenancyGrouping
{
    [DependsOn(
        typeof(AbpMultiTenancyModule)
        )]
    public class AbpTenancyGroupingModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<AbpTenancyGroupingOptions>(options =>
            {
                options.IsEnabled = true;
            });
            return Task.CompletedTask;
        }
    }
}
