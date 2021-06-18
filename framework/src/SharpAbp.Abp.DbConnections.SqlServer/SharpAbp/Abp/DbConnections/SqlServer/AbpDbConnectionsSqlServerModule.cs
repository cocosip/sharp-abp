using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.DbConnections.SqlServer
{
    [DependsOn(
        typeof(AbpDbConnectionsModule),
        typeof(AbpEntityFrameworkCoreSqlServerModule)
        )]
    public class AbpDbConnectionsSqlServerModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpDbConnectionsOptions>(options =>
            {
                options.DatabaseProviders.Add(DatabaseProvider.SqlServer);
            });
        }
    }
}
