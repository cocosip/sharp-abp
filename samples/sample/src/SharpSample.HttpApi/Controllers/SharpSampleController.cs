using SharpSample.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace SharpSample.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class SharpSampleController : AbpControllerBase
{
    protected SharpSampleController()
    {
        LocalizationResource = typeof(SharpSampleResource);
    }
}
