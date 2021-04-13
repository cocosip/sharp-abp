using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SharpAbp.Abp.MapTenancy;
using Volo.Abp.Caching;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement;

namespace SharpAbp.Abp.MapTenancyManagement
{
    [DependsOn(
        typeof(AbpCachingModule),
        typeof(AbpMapTenancyModule),
        typeof(AbpTenantManagementDomainModule),
        typeof(MapTenancyManagementDomainSharedModule)
        )]
    public class MapTenancyManagementDomainModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.Replace(ServiceDescriptor.Transient<IMapTenancyConfigurationProvider, DatabaseMapTenancyConfigurationProvider>());
        }
    }
}
