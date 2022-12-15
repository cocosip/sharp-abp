using System;
using System.Collections.Generic;
using System.Text;
using SharpSample.Localization;
using Volo.Abp.Application.Services;

namespace SharpSample;

/* Inherit your application services from this class.
 */
public abstract class SharpSampleAppService : ApplicationService
{
    protected SharpSampleAppService()
    {
        LocalizationResource = typeof(SharpSampleResource);
    }
}
