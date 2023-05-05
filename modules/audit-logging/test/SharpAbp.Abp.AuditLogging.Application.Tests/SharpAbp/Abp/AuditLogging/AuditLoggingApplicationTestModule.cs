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
        typeof(AuditLoggingApplicationModule),
        typeof(AbpAuditLoggingEntityFrameworkCoreModule),
        typeof(AbpTestBaseModule),
        typeof(AbpAutofacModule)
        )]
    public class AuditLoggingApplicationTestModule : AbpModule
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
