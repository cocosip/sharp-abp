using Volo.Abp.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;
using Volo.Abp.Uow;

namespace SharpAbp.Abp.AuditLogging
{
    [DependsOn(
        typeof(AbpEntityFrameworkCoreSqliteModule),
        typeof(AuditLoggingApplicationModule),
        typeof(AbpAuditLoggingEntityFrameworkCoreModule),
        typeof(AbpTestBaseModule),
        typeof(AbpAutofacModule)
        )]
    public class AuditLoggingApplicationTestModule : AbpModule
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
            using (var dbContext = new AbpAuditLoggingDbContext(new DbContextOptionsBuilder<AbpAuditLoggingDbContext>().UseSqlite(_connection).Options))
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


            Configure<AbpAuditingOptions>(options =>
            {
                options.ApplicationName = "AuditLoggingTest";
            });

        }
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            using var scope = context.ServiceProvider.CreateScope();
            AsyncHelper.RunSync(() =>
            {
                return scope.ServiceProvider
                 .GetRequiredService<AuditLoggingDataSeedContributor>()
                 .SeedAsync();
            });
        }


    }
}
