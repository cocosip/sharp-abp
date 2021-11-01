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


        // protected virtual IStringLocalizer CreateFileProviderLocalizer()
        // {
        //     if (LocalizationResource != null)
        //     {
        //         return StringLocalizerFactory.Create(LocalizationResource);
        //     }

        //     var localizer = StringLocalizerFactory.CreateDefaultOrNull();
        //     if (localizer == null)
        //     {
        //         throw new AbpException($"Set {nameof(LocalizationResource)} or define the default localization resource type (by configuring the {nameof(AbpLocalizationOptions)}.{nameof(AbpLocalizationOptions.DefaultResourceType)}) to be able to use the {nameof(L)} object!");
        //     }

        //     return localizer;
        // }

    }
}
