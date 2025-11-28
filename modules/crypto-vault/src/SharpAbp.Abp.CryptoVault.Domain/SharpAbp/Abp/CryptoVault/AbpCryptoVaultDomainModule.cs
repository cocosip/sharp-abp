using SharpAbp.Abp.Crypto;
using SharpAbp.Abp.Crypto.SM2;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.CryptoVault
{

    [DependsOn(
        typeof(AbpDddDomainModule),
        typeof(AbpCachingModule),
        typeof(AbpCryptoVaultDomainSharedModule),
        typeof(AbpCryptoModule)
        )]
    public class AbpCryptoVaultDomainModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }
        
        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<AbpCryptoVaultOptions>(options =>
            {
                options.RSACount = 20;
                options.RSAKeySize = 2048;
                options.SM2Count = 20;
                options.SM2Curve = Sm2EncryptionNames.CurveSm2p256v1;
            });

            return Task.CompletedTask;
        }

    }
}
