using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;
using Volo.Abp.Threading;

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
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddMongoDbContext<MapTenancyManagementMongoDbContext>(options =>
            {
                options.AddDefaultRepositories<IMapTenancyManagementMongoDbContext>();
                options.AddRepository<MapTenant, MongoDbMapTenantRepository>();
            });
            return Task.CompletedTask;
        }
    }
}
