using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.Uow;

namespace SharpAbp.Abp.OpenIddict
{
    [DependsOn(
         typeof(OpenIddictApplicationModule),
         typeof(AbpOpenIddictEntityFrameworkCoreModule),
         typeof(AbpPermissionManagementEntityFrameworkCoreModule),
         typeof(AbpTestBaseModule),
         typeof(AbpAutofacModule)
         )]
    public class OpenIddictApplicationTestModule : AbpModule
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
