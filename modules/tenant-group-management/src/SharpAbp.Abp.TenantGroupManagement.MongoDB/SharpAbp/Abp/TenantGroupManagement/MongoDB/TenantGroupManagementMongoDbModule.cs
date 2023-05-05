using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;
using Volo.Abp.Threading;

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
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddMongoDbContext<TenantGroupManagementMongoDbContext>(options =>
            {
                options.AddDefaultRepositories<ITenantGroupManagementMongoDbContext>();
                options.AddRepository<TenantGroup, MongoDbTenantGroupRepository>();
            });
            return Task.CompletedTask;
        }

    }
}
