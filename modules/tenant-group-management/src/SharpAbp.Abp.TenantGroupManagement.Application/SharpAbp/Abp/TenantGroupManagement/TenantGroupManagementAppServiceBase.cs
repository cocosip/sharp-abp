using SharpAbp.Abp.TenantGroupManagement.Localization;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.TenantGroupManagement
{
    public abstract class TenantGroupManagementAppServiceBase : ApplicationService
    {
        public TenantGroupManagementAppServiceBase()
        {
            LocalizationResource = typeof(TenantGroupManagementResource);
            ObjectMapperContext = typeof(TenantGroupManagementApplicationModule);
        }
    }
}
