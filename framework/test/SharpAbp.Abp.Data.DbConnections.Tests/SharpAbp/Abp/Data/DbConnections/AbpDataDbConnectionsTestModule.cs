using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.Data.DbConnections.MySQL;
using SharpAbp.Abp.Data.DbConnections.Oracle.Devart;
using SharpAbp.Abp.Data.DbConnections.PostgreSql;
using SharpAbp.Abp.Data.DbConnections.Sqlite;
using SharpAbp.Abp.Data.DbConnections.SqlServer;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Data.DbConnections
{
    [DependsOn(
        typeof(AbpDataDbConnectionsMySQLModule),
        typeof(AbpDataDbConnectionsPostgreSqlModule),
        typeof(AbpDataDbConnectionsSqlServerModule),
        typeof(AbpDataDbConnectionsOracleDevartModule),
        typeof(AbpDataDbConnectionsSqliteModule),
        typeof(AbpTestBaseModule),
        typeof(AbpAutofacModule)
      )]
    public class AbpDataDbConnectionsTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {

            var configuration = context.Services.GetConfiguration().GetSection("DataDbConnectionsOptions");

            Configure<AbpDataDbConnectionsOptions>(options =>
            {
                options.Configure(configuration);
            });
        }
    }
}
