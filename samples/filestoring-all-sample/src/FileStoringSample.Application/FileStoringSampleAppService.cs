using System;
using System.Collections.Generic;
using System.Text;
using FileStoringSample.Localization;
using Volo.Abp.Application.Services;

namespace FileStoringSample
{
    /* Inherit your application services from this class.
     */
    public abstract class FileStoringSampleAppService : ApplicationService
    {
        protected FileStoringSampleAppService()
        {
            LocalizationResource = typeof(FileStoringSampleResource);
        }
    }
}
