using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.DbConnectionsManagement.EntityFrameworkCore
{
    [DependsOn(
        typeof(DbConnectionsManagementDomainModule),
        typeof(AbpEntityFrameworkCoreModule)
        )]
    public class DbConnectionsManagementEntityFrameworkCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() =>
            {
                return ConfigureServicesAsync(context);
            });
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<DbConnectionsManagementDbContext>(options =>
            {
                options.AddDefaultRepositories<IDbConnectionsManagementDbContext>(includeAllEntities: true);
                options.AddRepository<DatabaseConnectionInfo, EfCoreDatabaseConnectionInfoRepository>();
            });
            return Task.CompletedTask;
        }


    }
}
