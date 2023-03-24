using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement;

namespace SharpAbp.Abp.TenantGroupManagement
{
    [DependsOn(
        typeof(TenantGroupManagementDomainModule),
        typeof(TenantGroupManagementApplicationContractsModule),
        typeof(AbpTenantManagementApplicationModule),
        typeof(AbpDddApplicationModule),
        typeof(AbpAutoMapperModule)
        )]
    public class TenantGroupManagementApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<TenantGroupManagementApplicationModule>();
            });

            context.Services.AddAutoMapperObjectMapper<TenantGroupManagementApplicationModule>();
        }
    }
}
