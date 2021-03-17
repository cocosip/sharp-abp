using SharpAbp.Abp.MapTenancyManagement.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public abstract class MapTenancyController : AbpController
    {
        protected MapTenancyController()
        {
            LocalizationResource = typeof(MapTenancyManagementResource);
        }
    }
}
