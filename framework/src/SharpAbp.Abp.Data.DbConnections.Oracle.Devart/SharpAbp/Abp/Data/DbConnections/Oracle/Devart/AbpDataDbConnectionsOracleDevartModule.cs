using Volo.Abp.EntityFrameworkCore.Oracle.Devart;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Data.DbConnections.Oracle.Devart
{
    [DependsOn(
        typeof(AbpDataDbConnectionsModule),
        typeof(AbpEntityFrameworkCoreOracleDevartModule)
        )]
    public class AbpDataDbConnectionsOracleDevartModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpDataDbConnectionsOptions>(options =>
            {
                options.DatabaseProviders.Add(DatabaseProvider.Oracle);
            });
        }
    }
}
