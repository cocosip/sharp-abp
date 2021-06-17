using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Data.DbConnections.Sqlite
{
    [DependsOn(
        typeof(AbpDataDbConnectionsModule),
        typeof(AbpEntityFrameworkCoreSqliteModule)
        )]
    public class AbpDataDbConnectionsSqliteModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpDataDbConnectionsOptions>(options =>
            {
                options.DatabaseProviders.Add(DatabaseProvider.Sqlite);
            });
        }
    }
}
