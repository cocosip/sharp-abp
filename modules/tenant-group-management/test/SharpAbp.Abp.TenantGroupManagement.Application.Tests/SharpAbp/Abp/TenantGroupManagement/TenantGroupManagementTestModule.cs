using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.TenantGroupManagement.EntityFrameworkCore;
using System;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Volo.Abp.Uow;

namespace SharpAbp.Abp.TenantGroupManagement
{
    [DependsOn(
        typeof(TenantGroupManagementApplicationModule),
        typeof(TenantGroupManagementEntityFrameworkCoreModule),
        typeof(AbpTenantManagementEntityFrameworkCoreModule),
        typeof(AbpTestBaseModule),
        typeof(AbpAutofacModule)
        )]
    public class TenantGroupManagementTestModule : AbpModule
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


            Configure<AbpDbConnectionOptions>(options =>
            {
                options.ConnectionStrings.Default = "123";
            });

        }
    }
}
