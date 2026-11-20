using Volo.Abp.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.SecurityLog;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.Uow;

namespace SharpAbp.Abp.Identity
{
    [DependsOn(
        typeof(AbpEntityFrameworkCoreSqliteModule),
         typeof(IdentityApplicationModule),
         typeof(AbpIdentityEntityFrameworkCoreModule),
         typeof(AbpSettingManagementEntityFrameworkCoreModule),
         typeof(AbpTestBaseModule),
         typeof(AbpAutofacModule)
         )]
    public class IdentityApplicationTestModule : AbpModule
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
            using (var dbContext = new IdentityDbContext(new DbContextOptionsBuilder<IdentityDbContext>().UseSqlite(_connection).Options))
            {
                dbContext.GetService<IRelationalDatabaseCreator>().CreateTables();
            }
            using (var dbContext = new SettingManagementDbContext(new DbContextOptionsBuilder<SettingManagementDbContext>().UseSqlite(_connection).Options))
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

            Configure<AbpSecurityLogOptions>(options =>
            {
                options.IsEnabled = true;
                options.ApplicationName = "App1";
            });

        }


    }
}