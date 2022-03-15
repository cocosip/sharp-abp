using AllApiSample.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace AllApiSample.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class AllApiSampleController : AbpControllerBase
{
    protected AllApiSampleController()
    {
        LocalizationResource = typeof(AllApiSampleResource);
    }
}
