using SharpAbp.Abp.CryptoVault.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace SharpAbp.Abp.CryptoVault
{
    public abstract class CryptoVaultController : AbpController
    {
        public CryptoVaultController()
        {
            LocalizationResource = typeof(AbpCryptoVaultResource);
        }
    }
}
