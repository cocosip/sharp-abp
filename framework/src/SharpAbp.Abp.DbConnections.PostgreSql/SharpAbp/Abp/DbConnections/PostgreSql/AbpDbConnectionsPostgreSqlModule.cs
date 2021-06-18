using Volo.Abp.EntityFrameworkCore.PostgreSql;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.DbConnections.PostgreSql
{
    [DependsOn(
          typeof(AbpDbConnectionsModule),
          typeof(AbpEntityFrameworkCorePostgreSqlModule)
          )]
    public class AbpDbConnectionsPostgreSqlModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpDbConnectionsOptions>(options =>
            {
                options.DatabaseProviders.Add(DatabaseProvider.PostgreSql);
            });
        }
    }
}
