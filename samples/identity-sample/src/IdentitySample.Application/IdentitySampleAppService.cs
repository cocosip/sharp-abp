using System;
using System.Collections.Generic;
using System.Text;
using IdentitySample.Localization;
using Volo.Abp.Application.Services;

namespace IdentitySample
{
    /* Inherit your application services from this class.
     */
    public abstract class IdentitySampleAppService : ApplicationService
    {
        protected IdentitySampleAppService()
        {
            LocalizationResource = typeof(IdentitySampleResource);
        }
    }
}
