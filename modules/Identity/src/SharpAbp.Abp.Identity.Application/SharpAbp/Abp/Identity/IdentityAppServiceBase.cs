using SharpAbp.Abp.Identity.Localization;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.Identity
{
    public class IdentityAppServiceBase : ApplicationService
    {
        protected IdentityAppServiceBase()
        {
            ObjectMapperContext = typeof(IdentityApplicationModule);
            LocalizationResource = typeof(IdentityResource);
        }
    }
}
