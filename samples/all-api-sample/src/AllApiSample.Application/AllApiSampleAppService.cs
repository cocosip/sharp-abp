using System;
using System.Collections.Generic;
using System.Text;
using AllApiSample.Localization;
using Volo.Abp.Application.Services;

namespace AllApiSample;

/* Inherit your application services from this class.
 */
public abstract class AllApiSampleAppService : ApplicationService
{
    protected AllApiSampleAppService()
    {
        LocalizationResource = typeof(AllApiSampleResource);
    }
}
