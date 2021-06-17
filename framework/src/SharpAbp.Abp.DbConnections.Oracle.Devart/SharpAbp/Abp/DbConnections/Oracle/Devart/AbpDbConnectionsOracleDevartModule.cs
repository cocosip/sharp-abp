using Volo.Abp.EntityFrameworkCore.Oracle.Devart;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.DbConnections.Oracle.Devart
{
    [DependsOn(
        typeof(AbpDbConnectionsModule),
        typeof(AbpEntityFrameworkCoreOracleDevartModule)
        )]
    public class AbpDbConnectionsOracleDevartModule : AbpModule
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
