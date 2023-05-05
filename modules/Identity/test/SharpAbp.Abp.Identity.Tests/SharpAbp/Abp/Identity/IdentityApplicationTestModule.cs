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
         typeof(IdentityApplicationModule),
         typeof(AbpIdentityEntityFrameworkCoreModule),
         typeof(AbpSettingManagementEntityFrameworkCoreModule),
         typeof(AbpTestBaseModule),
         typeof(AbpAutofacModule)
         )]
    public class IdentityApplicationTestModule : AbpModule
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

            Configure<AbpSecurityLogOptions>(options =>
            {
                options.IsEnabled = true;
                options.ApplicationName = "App1";
            });

        }
 

    }
}