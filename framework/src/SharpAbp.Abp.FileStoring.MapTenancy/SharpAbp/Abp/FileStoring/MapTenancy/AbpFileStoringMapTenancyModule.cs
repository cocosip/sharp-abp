using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;
using SharpAbp.Abp.MapTenancy;

namespace SharpAbp.Abp.FileStoring.MapTenancy
{
    [DependsOn(
        typeof(AbpFileStoringModule),
        typeof(AbpMapTenancyModule)
        )]
    public class AbpFileStoringMapTenancyModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<AbpFileStoringMapTenancyOptions>(options => { });

            Configure<AbpFilePathContextResolveOptions>(options =>
            {
                options.Contributors.Insert(0, new MapTenancyFilePathContextResolveContributor());
            });

            return Task.CompletedTask;
        }
    }
}
