using Volo.Abp.EntityFrameworkCore.Oracle;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Data.DbConnections.Oracle
{
    [DependsOn(
        typeof(AbpDataDbConnectionsModule),
        typeof(AbpEntityFrameworkCoreOracleModule)
        )]
    public class AbpDataDbConnectionsOracleModule : AbpModule
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
