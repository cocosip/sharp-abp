using FileStoringSample.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace FileStoringSample.Controllers
{
    /* Inherit your controllers from this class.
     */
    public abstract class FileStoringSampleController : AbpController
    {
        protected FileStoringSampleController()
        {
            LocalizationResource = typeof(FileStoringSampleResource);
        }
    }
}