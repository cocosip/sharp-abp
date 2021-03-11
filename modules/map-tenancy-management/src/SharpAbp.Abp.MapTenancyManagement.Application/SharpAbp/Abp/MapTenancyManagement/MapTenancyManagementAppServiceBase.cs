using SharpAbp.Abp.MapTenancyManagement.Localization;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public abstract class MapTenancyManagementAppServiceBase : ApplicationService
    {
        protected MapTenancyManagementAppServiceBase()
        {
            LocalizationResource = typeof(MapTenancyManagementResource);
            ObjectMapperContext = typeof(MapTenancyManagementApplicationModule);
        }
    }
}
