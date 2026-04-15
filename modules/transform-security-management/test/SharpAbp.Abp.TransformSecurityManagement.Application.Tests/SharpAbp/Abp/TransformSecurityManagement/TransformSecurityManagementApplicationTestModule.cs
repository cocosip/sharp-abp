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
        typeof(AbpTransformSecurityManagementApplicationModule),
        typeof(AbpTransformSecurityManagementEntityFrameworkCoreModule),
        typeof(AbpCryptoVaultApplicationModule),
        typeof(AbpCryptoVaultEntityFrameworkCoreModule),
        typeof(AbpTestBaseModule),
        typeof(AbpAutofacModule)
        )]
    public class TransformSecurityManagementApplicationTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAlwaysAllowAuthorization();
            context.Services.AddEntityFrameworkInMemoryDatabase();

            var databaseName = Guid.NewGuid().ToString();

            Configure<AbpDbContextOptions>(options =>
            {
                options.Configure(configurationContext =>
                {
                    configurationContext.DbContextOptions.UseInMemoryDatabase(databaseName);
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
