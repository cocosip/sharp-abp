using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.DbConnections.MySQL;
using SharpAbp.Abp.DbConnections.Oracle.Drvart;
using SharpAbp.Abp.DbConnections.PostgreSql;
using SharpAbp.Abp.DbConnections.Sqlite;
using SharpAbp.Abp.DbConnections.SqlServer;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.DbConnections
{
    [DependsOn(
        typeof(AbpDbConnectionsMySQLModule),
        typeof(AbpDbConnectionsPostgreSqlModule),
        typeof(AbpDbConnectionsSqlServerModule),
        typeof(AbpDbConnectionsOracleDevartModule),
        typeof(AbpDbConnectionsSqliteModule),
        typeof(AbpTestBaseModule),
        typeof(AbpAutofacModule)
      )]
    public class AbpDbConnectionsTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {

            var configuration = context.Services.GetConfiguration();

            Configure<AbpDbConnectionsOptions>(options =>
            {
                options.Configure(configuration);
            });
        }
    }
}
