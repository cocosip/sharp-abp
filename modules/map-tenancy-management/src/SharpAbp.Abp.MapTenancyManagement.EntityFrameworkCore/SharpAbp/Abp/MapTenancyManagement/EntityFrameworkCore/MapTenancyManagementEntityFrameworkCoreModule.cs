using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace SharpAbp.Abp.MapTenancyManagement.EntityFrameworkCore
{
    [DependsOn(
        typeof(MapTenancyManagementDomainModule),
        typeof(AbpTenantManagementEntityFrameworkCoreModule),
        typeof(AbpEntityFrameworkCoreModule)
        )]
    public class MapTenancyManagementEntityFrameworkCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<MapTenancyManagementDbContext>(options =>
            {
                options.AddDefaultRepositories<IMapTenancyManagementDbContext>();
                options.AddRepository<MapTenant, EfCoreMapTenantRepository>();
            });
        }
    }
}