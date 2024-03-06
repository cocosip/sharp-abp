using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.DbConnectionsManagement.MongoDB
{
    [DependsOn(
        typeof(DbConnectionsManagementDomainModule),
        typeof(AbpMongoDbModule)
        )]
    public class DbConnectionsManagementMongoDbModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddMongoDbContext<DbConnectionsManagementMongoDbContext>(options =>
            {
                options.AddDefaultRepositories<IDbConnectionsManagementMongoDbContext>();
                options.AddRepository<DatabaseConnectionInfo, MongoDbDatabaseConnectionInfoRepository>();
            });
            return Task.CompletedTask;
        }
    }
}
