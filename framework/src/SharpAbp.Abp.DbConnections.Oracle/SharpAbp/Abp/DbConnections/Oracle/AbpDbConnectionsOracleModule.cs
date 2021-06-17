using Volo.Abp.EntityFrameworkCore.Oracle;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.DbConnections.Oracle
{
    [DependsOn(
        typeof(AbpDbConnectionsModule),
        typeof(AbpEntityFrameworkCoreOracleModule)
        )]
    public class AbpDbConnectionsOracleModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpDbConnectionsOptions>(options =>
            {
                options.DatabaseProviders.Configure(DatabaseProvider.Oracle, c =>
                {
                    c.DatabaseProvider = DatabaseProvider.Oracle;
                });
            });
        }
    }
}
