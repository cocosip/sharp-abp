using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.TenancyGrouping;
using Volo.Abp.AutoMapper;
using Volo.Abp.Caching;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement;

namespace SharpAbp.Abp.TenantGroupManagement
{
    [DependsOn(
        typeof(AbpCachingModule),
        typeof(AbpTenancyGroupingModule),
        typeof(AbpTenantManagementDomainModule),
        typeof(TenantGroupManagementDomainSharedModule)
        )]
    public class TenantGroupManagementDomainModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAutoMapperObjectMapper<TenantGroupManagementDomainModule>();

            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddProfile<AbpTenantGroupManagementDomainMappingProfile>(validate: true);
            });

        }
    }
}
