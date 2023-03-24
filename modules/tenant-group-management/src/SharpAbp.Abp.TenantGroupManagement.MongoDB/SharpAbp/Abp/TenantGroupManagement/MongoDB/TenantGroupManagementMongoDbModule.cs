using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.TenantGroupManagement.MongoDB
{
    [DependsOn(
        typeof(TenantGroupManagementDomainModule),
        typeof(AbpMongoDbModule)
        )]
    public class TenantGroupManagementMongoDbModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddMongoDbContext<TenantGroupManagementMongoDbContext>(options =>
            {
                options.AddDefaultRepositories<ITenantGroupManagementMongoDbContext>();
                options.AddRepository<TenantGroup, MongoDbTenantGroupRepository>();
            });
        }
    }
}
