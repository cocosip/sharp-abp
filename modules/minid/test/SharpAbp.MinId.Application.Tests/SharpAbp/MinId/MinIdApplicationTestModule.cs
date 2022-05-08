using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SharpAbp.MinId.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Guids;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;
using Volo.Abp.Uow;

namespace SharpAbp.MinId
{
    [DependsOn(
      typeof(MinIdApplicationModule),
      typeof(MinIdEntityFrameworkCoreModule),
      typeof(AbpTestBaseModule),
      typeof(AbpAutofacModule)
      )]
    public class MinIdApplicationTestModule : AbpModule
    {

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAlwaysAllowAuthorization();

            context.Services.Replace(ServiceDescriptor.Transient<ISegmentIdService, TestSegmentIdService>());

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


        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            //Create test data
            AsyncHelper.RunSync(() =>
            {
                return CreateDataAsync(context);
            });
        }

        private async Task CreateDataAsync(ApplicationInitializationContext context)
        {
            var guidGenerator = context.ServiceProvider.GetRequiredService<IGuidGenerator>();
            var minIdInfoRepository = context.ServiceProvider.GetRequiredService<IMinIdInfoRepository>();
            var minIdTokenRepository = context.ServiceProvider.GetRequiredService<IMinIdTokenRepository>();

            var minIdInfo = await minIdInfoRepository.FindByBizTypeAsync("default");
            if (minIdInfo == null)
            {
                minIdInfo = new MinIdInfo(guidGenerator.Create(), "default", 0, 100, 1, 0);
                await minIdInfoRepository.InsertAsync(minIdInfo);
            }

            var minIdToken = await minIdTokenRepository.FindByTokenAsync("default", "123456");
            if (minIdInfo == null)
            {
                minIdToken = new MinIdToken(guidGenerator.Create(), "default", "123456", "");
                await minIdTokenRepository.InsertAsync(minIdToken);
            }
        }

    }
}
