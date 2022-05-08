using MinIdApp.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace MinIdApp.Controllers
{
    /* Inherit your controllers from this class.
     */
    public abstract class MinIdAppController : AbpController
    {
        protected MinIdAppController()
        {
            LocalizationResource = typeof(MinIdAppResource);
        }
    }
}