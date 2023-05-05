using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Volo.Abp.Threading;

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
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<MapTenancyManagementDbContext>(options =>
            {
                options.AddDefaultRepositories<IMapTenancyManagementDbContext>();
                options.AddRepository<MapTenant, EfCoreMapTenantRepository>();
            });
            return Task.CompletedTask;
        }
    }
}