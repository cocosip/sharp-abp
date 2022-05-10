using Volo.Abp.EntityFrameworkCore.Oracle.Devart;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.DbConnections.Oracle.Drvart
{
    [DependsOn(
        typeof(AbpDbConnectionsModule)
        //typeof(AbpEntityFrameworkCoreOracleDevartModule)
        )]
    public class AbpDbConnectionsOracleDevartModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpDbConnectionsOptions>(options =>
            {
                options.DatabaseProviders.Add(DatabaseProvider.Oracle);
            });
        }
    }
}
