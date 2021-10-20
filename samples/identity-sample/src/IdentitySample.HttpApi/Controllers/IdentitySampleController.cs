using IdentitySample.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace IdentitySample.Controllers
{
    /* Inherit your controllers from this class.
     */
    public abstract class IdentitySampleController : AbpController
    {
        protected IdentitySampleController()
        {
            LocalizationResource = typeof(IdentitySampleResource);
        }
    }
}