using SharpAbp.WebSample.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace SharpAbp.WebSample.Controllers
{
    /* Inherit your controllers from this class.
     */
    public abstract class WebSampleController : AbpController
    {
        protected WebSampleController()
        {
            LocalizationResource = typeof(WebSampleResource);
        }
    }
}