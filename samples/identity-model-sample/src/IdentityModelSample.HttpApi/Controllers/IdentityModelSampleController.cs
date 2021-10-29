using IdentityModelSample.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace IdentityModelSample.Controllers
{
    /* Inherit your controllers from this class.
     */
    public abstract class IdentityModelSampleController : AbpController
    {
        protected IdentityModelSampleController()
        {
            LocalizationResource = typeof(IdentityModelSampleResource);
        }
    }
}