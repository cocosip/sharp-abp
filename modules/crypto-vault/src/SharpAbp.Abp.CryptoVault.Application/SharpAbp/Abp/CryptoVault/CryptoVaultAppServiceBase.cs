using SharpAbp.Abp.CryptoVault.Localization;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.CryptoVault
{
    public abstract class CryptoVaultAppServiceBase : ApplicationService
    {
        protected CryptoVaultAppServiceBase()
        {
            ObjectMapperContext = typeof(AbpCryptoVaultApplicationModule);
            LocalizationResource = typeof(AbpCryptoVaultResource);
        }
    }
}
