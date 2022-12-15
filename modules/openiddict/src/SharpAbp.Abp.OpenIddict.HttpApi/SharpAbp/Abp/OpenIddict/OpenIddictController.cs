using SharpAbp.Abp.OpenIddict.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace SharpAbp.Abp.OpenIddict
{
    public abstract class OpenIddictController : AbpController
    {
        public OpenIddictController()
        {
            LocalizationResource = typeof(OpenIddictResource);
        }
    }
}
