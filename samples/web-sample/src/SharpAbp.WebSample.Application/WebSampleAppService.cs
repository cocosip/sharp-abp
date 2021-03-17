using System;
using System.Collections.Generic;
using System.Text;
using SharpAbp.WebSample.Localization;
using Volo.Abp.Application.Services;

namespace SharpAbp.WebSample
{
    /* Inherit your application services from this class.
     */
    public abstract class WebSampleAppService : ApplicationService
    {
        protected WebSampleAppService()
        {
            LocalizationResource = typeof(WebSampleResource);
        }
    }
}
