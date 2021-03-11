using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.MapTenancyManagement.MongoDB
{
    [DependsOn(
        typeof(MapTenancyManagementDomainModule),
        typeof(AbpMongoDbModule)
        )]
    public class MapTenancyManagementMongoDbModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddMongoDbContext<MapTenancyManagementMongoDbContext>(options =>
            {
                options.AddDefaultRepositories<IMapTenancyManagementMongoDbContext>();
                options.AddRepository<MapTenant, MongoDbMapTenantRepository>();
            });
        }
    }
}
