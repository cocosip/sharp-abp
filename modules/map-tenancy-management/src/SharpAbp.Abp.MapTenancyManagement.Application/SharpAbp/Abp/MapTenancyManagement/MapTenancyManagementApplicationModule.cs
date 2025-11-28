using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.Application;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.MapTenancyManagement
{
    [DependsOn(
        typeof(MapTenancyManagementDomainModule),
        typeof(MapTenancyManagementApplicationContractsModule),
        typeof(AbpTenantManagementApplicationModule),
        typeof(AbpDddApplicationModule),
        typeof(AbpMapperlyModule)
        )]
    public class MapTenancyManagementApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddMapperlyObjectMapper<MapTenancyManagementApplicationModule>();
            return Task.CompletedTask;
        }
    }
}
