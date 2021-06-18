using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.DbConnections.Sqlite
{
    [DependsOn(
        typeof(AbpDbConnectionsModule),
        typeof(AbpEntityFrameworkCoreSqliteModule)
        )]
    public class AbpDbConnectionsSqliteModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpDbConnectionsOptions>(options =>
            {
                options.DatabaseProviders.Add(DatabaseProvider.Sqlite);
            });
        }
    }
}
