using Microsoft.Extensions.Localization;
using SharpAbp.Abp.FileStoringManagement.Localization;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.FileStoringManagement
{
    public abstract class FileStoringManagementAppServiceBase : ApplicationService
    {
        protected FileStoringManagementAppServiceBase()
        {
            ObjectMapperContext = typeof(FileStoringManagementApplicationModule);
            LocalizationResource = typeof(FileStoringManagementResource);
        }

    }
}
