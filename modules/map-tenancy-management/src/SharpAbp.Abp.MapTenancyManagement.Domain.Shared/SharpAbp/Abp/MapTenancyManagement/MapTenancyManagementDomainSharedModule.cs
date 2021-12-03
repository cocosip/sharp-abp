using SharpAbp.Abp.MapTenancyManagement.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace SharpAbp.Abp.MapTenancyManagement
{
    [DependsOn(
        typeof(AbpValidationModule),
        typeof(AbpTenantManagementDomainSharedModule)
        )]
    public class MapTenancyManagementDomainSharedModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<MapTenancyManagementDomainSharedModule>();
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Add<MapTenancyManagementResource>("en")
                    .AddBaseTypes(typeof(AbpValidationResource))
                    .AddVirtualJson("/SharpAbp/Abp/MapTenancyManagement/Localization/Resources");
            });

            Configure<AbpExceptionLocalizationOptions>(options =>
            {
                options.MapCodeNamespace("MapTenancyManagement", typeof(MapTenancyManagementResource));
            });
        }
    }
}
