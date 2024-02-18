using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.CryptoVault
{
    [DependsOn(
        typeof(AbpCryptoVaultDomainSharedModule),
        typeof(AbpDddApplicationContractsModule),
        typeof(AbpAuthorizationModule)
        )]
    public class AbpCryptoVaultApplicationContractsModule : AbpModule
    {

    }
}
