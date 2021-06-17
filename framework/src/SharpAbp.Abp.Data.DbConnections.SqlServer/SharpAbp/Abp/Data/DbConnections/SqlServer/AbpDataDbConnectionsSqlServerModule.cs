using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Data.DbConnections.SqlServer
{
    [DependsOn(
        typeof(AbpDataDbConnectionsModule),
        typeof(AbpEntityFrameworkCoreSqlServerModule)
        )]
    public class AbpDataDbConnectionsSqlServerModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpDataDbConnectionsOptions>(options =>
            {
                options.DatabaseProviders.Add(DatabaseProvider.SqlServer);
            });
        }
    }
}
