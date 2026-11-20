using Volo.Abp.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.Crypto.RSA;
using SharpAbp.Abp.CryptoVault;
using SharpAbp.Abp.CryptoVault.EntityFrameworkCore;
using SharpAbp.Abp.TransformSecurity;
using SharpAbp.Abp.TransformSecurityManagement.EntityFrameworkCore;
using System;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;

namespace SharpAbp.Abp.TransformSecurityManagement
{
    [DependsOn(
        typeof(AbpEntityFrameworkCoreSqliteModule),
        typeof(AbpTransformSecurityManagementApplicationModule),
        typeof(AbpTransformSecurityManagementEntityFrameworkCoreModule),
        typeof(AbpCryptoVaultApplicationModule),
        typeof(AbpCryptoVaultEntityFrameworkCoreModule),
        typeof(AbpTestBaseModule),
        typeof(AbpAutofacModule)
        )]
    public class TransformSecurityManagementApplicationTestModule : AbpModule
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
            using (var dbContext = new AbpTransformSecurityManagementDbContext(new DbContextOptionsBuilder<AbpTransformSecurityManagementDbContext>().UseSqlite(_connection).Options))
            {
                dbContext.GetService<IRelationalDatabaseCreator>().CreateTables();
            }
            using (var dbContext = new AbpCryptoVaultDbContext(new DbContextOptionsBuilder<AbpCryptoVaultDbContext>().UseSqlite(_connection).Options))
            {
                dbContext.GetService<IRelationalDatabaseCreator>().CreateTables();
            }

            Configure<AbpDbConnectionOptions>(options =>
            {
                options.ConnectionStrings.Default = _connection.ConnectionString;
            });

            Configure<AbpDbContextOptions>(options =>
            {
                options.Configure(configurationContext =>
                {
                    configurationContext.UseSqlite();
                });
            });

            Configure<AbpUnitOfWorkDefaultOptions>(options =>
            {
                options.TransactionBehavior = UnitOfWorkTransactionBehavior.Disabled;
            });

            Configure<AbpTransformSecurityOptions>(options =>
            {
                options.IsEnabled = true;
                options.EncryptionAlgo = AbpTransformSecurityNames.RSA;
                options.Expires = TimeSpan.FromMinutes(30);
                options.BizTypes.Clear();
                options.BizTypes.AddRange(["Login", "Payment"]);
            });

            Configure<AbpTransformSecurityRSAOptions>(options =>
            {
                options.KeySize = 2048;
                options.Padding = RSAPaddingNames.PKCS1Padding;
            });
        }
    }
}
