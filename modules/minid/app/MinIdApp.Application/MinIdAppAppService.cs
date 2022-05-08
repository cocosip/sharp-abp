using System;
using System.Collections.Generic;
using System.Text;
using MinIdApp.Localization;
using Volo.Abp.Application.Services;

namespace MinIdApp
{
    /* Inherit your application services from this class.
     */
    public abstract class MinIdAppAppService : ApplicationService
    {
        protected MinIdAppAppService()
        {
            LocalizationResource = typeof(MinIdAppResource);
        }
    }
}
