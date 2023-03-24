using SharpAbp.Abp.TenantGroupManagement.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace SharpAbp.Abp.TenantGroupManagement
{
    public abstract class TenantGroupManagementController : AbpController
    {
        public TenantGroupManagementController()
        {
            LocalizationResource = typeof(TenantGroupManagementResource);
        }
    }
}
