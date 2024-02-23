using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.CryptoVault.MongoDB
{
    [DependsOn(
        typeof(AbpCryptoVaultDomainModule),
        typeof(AbpMongoDbModule)
        )]
    public class AbpCryptoVaultMongoDbModule : AbpModule
    {

    }
}
