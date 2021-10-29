using System;
using System.Collections.Generic;
using System.Text;
using IdentityModelSample.Localization;
using Volo.Abp.Application.Services;

namespace IdentityModelSample
{
    /* Inherit your application services from this class.
     */
    public abstract class IdentityModelSampleAppService : ApplicationService
    {
        protected IdentityModelSampleAppService()
        {
            LocalizationResource = typeof(IdentityModelSampleResource);
        }
    }
}
