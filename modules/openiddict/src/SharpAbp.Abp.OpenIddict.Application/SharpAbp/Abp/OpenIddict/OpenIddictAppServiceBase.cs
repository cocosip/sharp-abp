using SharpAbp.Abp.OpenIddict.Localization;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.OpenIddict
{
    public abstract class OpenIddictAppServiceBase : ApplicationService
    {
        protected OpenIddictAppServiceBase()
        {
            ObjectMapperContext = typeof(OpenIddictApplicationModule);
            LocalizationResource = typeof(OpenIddictResource);
        }
    }
}
