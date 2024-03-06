using SharpAbp.Abp.CryptoVault.Localization;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.TransformSecurityManagement
{
    public abstract class TransformSecurityManagementAppServiceBase : ApplicationService
    {
        public TransformSecurityManagementAppServiceBase()
        {
            ObjectMapperContext = typeof(AbpTransformSecurityManagementApplicationModule);
            LocalizationResource = typeof(AbpCryptoVaultResource);
        }

    }
}
