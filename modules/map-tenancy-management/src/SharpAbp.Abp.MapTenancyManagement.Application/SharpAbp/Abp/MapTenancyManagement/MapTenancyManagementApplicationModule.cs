using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement;

namespace SharpAbp.Abp.MapTenancyManagement
{
    [DependsOn(
        typeof(MapTenancyManagementDomainModule),
        typeof(MapTenancyManagementApplicationContractsModule),
        typeof(AbpTenantManagementApplicationModule),
        typeof(AbpDddApplicationModule),
        typeof(AbpAutoMapperModule)
        )]
    public class MapTenancyManagementApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<MapTenancyManagementApplicationModule>();
            });

            context.Services.AddAutoMapperObjectMapper<MapTenancyManagementApplicationModule>();
        }
    }
}
