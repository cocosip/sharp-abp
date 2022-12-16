using Localization.Resources.AbpUi;
using SharpAbp.Abp.Account;
using SharpAbp.Abp.DbConnectionsManagement;
using SharpAbp.Abp.FileStoringManagement;
using SharpAbp.Abp.Identity;
using SharpAbp.Abp.MapTenancyManagement;
using SharpAbp.Abp.OpenIddict;
using SharpSample.Localization;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.HttpApi;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;

namespace SharpSample;

[DependsOn(
    typeof(SharpSampleApplicationContractsModule),
    typeof(AccountHttpApiModule),
    typeof(IdentityHttpApiModule),
    typeof(OpenIddictHttpApiModule),
    typeof(MapTenancyManagementHttpApiModule),
    typeof(DbConnectionsManagementHttpApiModule),
    typeof(FileStoringManagementHttpApiModule),
    typeof(AbpPermissionManagementHttpApiModule),
    typeof(AbpTenantManagementHttpApiModule),
    typeof(AbpFeatureManagementHttpApiModule),
    typeof(AbpSettingManagementHttpApiModule)
    )]
public class SharpSampleHttpApiModule : AbpModule
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
                .Get<SharpSampleResource>()
                .AddBaseTypes(
                    typeof(AbpUiResource)
                );
        });
    }
}
