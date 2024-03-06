using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.CryptoVault.MongoDB
{
    [DependsOn(
        typeof(AbpCryptoVaultDomainModule),
        typeof(AbpMongoDbModule)
        )]
    public class AbpCryptoVaultMongoDbModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }
        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddMongoDbContext<AbpCryptoVaultMongoDbContext>(options =>
            {
                options.AddDefaultRepositories<IAbpCryptoVaultMongoDbContext>();
                options.AddRepository<RSACreds, MongoDbRSACredsRepository>();
                options.AddRepository<SM2Creds, MongoDbSM2CredsRepository>();
            });
            return Task.CompletedTask;
        }
    }
}
