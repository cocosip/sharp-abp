using SharpAbp.Abp.AuditLogging;
using SharpAbp.Abp.DbConnectionsManagement;
using SharpAbp.Abp.FileStoringManagement;
using SharpAbp.Abp.Identity;
using SharpAbp.Abp.MapTenancyManagement;
using SharpAbp.Abp.OpenIddict;
using SharpAbp.MinId;
using SharpSample.Localization;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace SharpSample;

[DependsOn(
    typeof(AbpBackgroundJobsDomainSharedModule),
    typeof(AbpFeatureManagementDomainSharedModule),
    typeof(AbpPermissionManagementDomainSharedModule),
    typeof(AbpSettingManagementDomainSharedModule),
    typeof(AbpTenantManagementDomainSharedModule),
    typeof(IdentityDomainSharedModule),
    typeof(OpenIddictDomainSharedModule),
    typeof(AuditLoggingDomainSharedModule),
    typeof(MapTenancyManagementDomainSharedModule),
    typeof(FileStoringManagementDomainSharedModule),
    typeof(DbConnectionsManagementDomainSharedModule),
    typeof(MinIdDomainSharedModule)
    )]
public class SharpSampleDomainSharedModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        SharpSampleGlobalFeatureConfigurator.Configure();
        SharpSampleModuleExtensionConfigurator.Configure();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<SharpSampleDomainSharedModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<SharpSampleResource>("en")
                .AddBaseTypes(typeof(AbpValidationResource))
                .AddVirtualJson("/Localization/SharpSample");

            options.DefaultResourceType = typeof(SharpSampleResource);
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("SharpSample", typeof(SharpSampleResource));
        });
    }
}
