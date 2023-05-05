using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

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
            context.Services.AddAbpDbContext<DbConnectionsManagementDbContext>(options =>
            {
                options.AddDefaultRepositories<IDbConnectionsManagementDbContext>(includeAllEntities: true);
                options.AddRepository<DatabaseConnectionInfo, EfCoreDatabaseConnectionInfoRepository>();
            });
        }
    }
}
