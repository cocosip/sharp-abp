using Localization.Resources.AbpUi;
using AllApiSample.Localization;
using Volo.Abp.Account;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.HttpApi;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using SharpAbp.Abp.Account;
using SharpAbp.Abp.MapTenancyManagement;
using SharpAbp.Abp.Identity;
using SharpAbp.Abp.IdentityServer;
using SharpAbp.Abp.DbConnectionsManagement;
using SharpAbp.Abp.FileStoringManagement;

namespace AllApiSample;

[DependsOn(
    typeof(AllApiSampleApplicationContractsModule),
    typeof(AccountHttpApiModule),
    typeof(IdentityHttpApiModule),
    typeof(IdentityServerHttpApiModule),
    typeof(AbpPermissionManagementHttpApiModule),
    typeof(MapTenancyManagementHttpApiModule),
    typeof(AbpFeatureManagementHttpApiModule),
    typeof(AbpSettingManagementHttpApiModule),
    typeof(DbConnectionsManagementHttpApiModule),
    typeof(FileStoringManagementHttpApiModule)
    )]
public class AllApiSampleHttpApiModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        ConfigureLocalization();
    }

    private void ConfigureLocalization()
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<AllApiSampleResource>()
                .AddBaseTypes(
                    typeof(AbpUiResource)
                );
        });
    }
}
