using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.DbConnections.MySQL;
using SharpAbp.Abp.DbConnections.PostgreSql;
using SharpAbp.Abp.DbConnections.Sqlite;
using SharpAbp.Abp.DbConnections.SqlServer;
using SharpAbp.Abp.DbConnectionsManagement.EntityFrameworkCore;
using System;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    [DependsOn(
       typeof(DbConnectionsManagementApplicationModule),
       typeof(DbConnectionsManagementEntityFrameworkCoreModule),
       typeof(AbpDbConnectionsMySQLModule),
       typeof(AbpDbConnectionsPostgreSqlModule),
       typeof(AbpDbConnectionsSqlServerModule),
       //typeof(AbpDbConnectionsOracleDevartModule),
       typeof(AbpDbConnectionsSqliteModule),
       typeof(AbpTestBaseModule),
       typeof(AbpAutofacModule)
       )]
    public class DbConnectionsManagementTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAlwaysAllowAuthorization();
            context.Services.AddEntityFrameworkInMemoryDatabase();

            var databaseName = Guid.NewGuid().ToString();
            Configure<AbpDbContextOptions>(options =>
            {
                options.Configure(abpDbContextConfigurationContext =>
                {
                    abpDbContextConfigurationContext.DbContextOptions.UseInMemoryDatabase(databaseName);
                });
            });

            Configure<AbpUnitOfWorkDefaultOptions>(options =>
            {
                options.TransactionBehavior = UnitOfWorkTransactionBehavior.Disabled; //EF in-memory database does not support transactions
            });
        }
    }
}
