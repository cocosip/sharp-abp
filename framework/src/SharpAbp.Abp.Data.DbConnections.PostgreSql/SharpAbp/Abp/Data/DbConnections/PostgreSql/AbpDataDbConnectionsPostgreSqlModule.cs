using Volo.Abp.EntityFrameworkCore.PostgreSql;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Data.DbConnections.PostgreSql
{
    [DependsOn(
        typeof(AbpDataDbConnectionsModule),
        typeof(AbpEntityFrameworkCorePostgreSqlModule)
        )]
    public class AbpDataDbConnectionsPostgreSqlModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpDataDbConnectionsOptions>(options =>
            {
                options.DatabaseProviders.Add(DatabaseProvider.MySql);
            });
        }
    }
}
