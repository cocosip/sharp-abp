using SharpAbp.Abp.FileStoringManagement.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace SharpAbp.Abp.FileStoringManagement
{
    public abstract class FileStoringController : AbpController
    {
        protected FileStoringController()
        {
            LocalizationResource = typeof(FileStoringManagementResource);
        }
    }
}
