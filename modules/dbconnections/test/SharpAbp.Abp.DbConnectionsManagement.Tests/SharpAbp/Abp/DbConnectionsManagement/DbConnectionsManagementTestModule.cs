using Volo.Abp.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.DbConnections.MySQL;
using SharpAbp.Abp.DbConnections.Oracle;
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
        typeof(AbpEntityFrameworkCoreSqliteModule),
       typeof(DbConnectionsManagementApplicationModule),
       typeof(DbConnectionsManagementEntityFrameworkCoreModule),
       typeof(AbpDbConnectionsMySQLModule),
       typeof(AbpDbConnectionsPostgreSqlModule),
       typeof(AbpDbConnectionsSqlServerModule),
       typeof(AbpDbConnectionsOracleModule),
       typeof(AbpDbConnectionsSqliteModule),
       typeof(AbpTestBaseModule),
       typeof(AbpAutofacModule)
       )]
    public class DbConnectionsManagementTestModule : AbpModule
    {
        private SqliteConnection _connection;

        public override void OnApplicationShutdown(ApplicationShutdownContext context)
        {
            _connection.Dispose();
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAlwaysAllowAuthorization();


            _connection = new SqliteConnection($"Data Source={Guid.NewGuid():N};Mode=Memory;Cache=Shared");
            _connection.Open();
            using (var dbContext = new DbConnectionsManagementDbContext(new DbContextOptionsBuilder<DbConnectionsManagementDbContext>().UseSqlite(_connection).Options))
            {
                dbContext.GetService<IRelationalDatabaseCreator>().CreateTables();
            }
            Configure<AbpDbConnectionOptions>(options =>
            {
                options.ConnectionStrings.Default = _connection.ConnectionString;
            });

            Configure<AbpDbContextOptions>(options =>
            {
                options.Configure(abpDbContextConfigurationContext =>
                {
                    abpDbContextConfigurationContext.UseSqlite();
                });
            });
        }
    }
}
