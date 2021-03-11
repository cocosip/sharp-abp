using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.MapTenancyManagement.EntityFrameworkCore
{
    [DependsOn(
        typeof(MapTenancyManagementDomainModule),
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