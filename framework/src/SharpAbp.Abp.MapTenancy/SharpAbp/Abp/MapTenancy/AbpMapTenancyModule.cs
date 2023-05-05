using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.MapTenancy
{
    [DependsOn(
        typeof(AbpMultiTenancyModule)
        )]
    public class AbpMapTenancyModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<AbpMapTenancyOptions>(options => { });
            return Task.CompletedTask;
        }
    }
}
