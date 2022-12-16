using SharpAbp.Abp.Account;
using SharpAbp.Abp.DbConnectionsManagement;
using SharpAbp.Abp.FileStoringManagement;
using SharpAbp.Abp.Identity;
using SharpAbp.Abp.MapTenancyManagement;
using SharpAbp.Abp.OpenIddict;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;

namespace SharpSample;

[DependsOn(
    typeof(SharpSampleDomainSharedModule),
    typeof(AccountApplicationContractsModule),
    typeof(AbpFeatureManagementApplicationContractsModule),
    typeof(IdentityApplicationContractsModule),
    typeof(OpenIddictApplicationContractsModule),
    typeof(MapTenancyManagementApplicationContractsModule),
    typeof(DbConnectionsManagementApplicationContractsModule),
    typeof(FileStoringManagementApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationContractsModule),
    typeof(AbpSettingManagementApplicationContractsModule),
    typeof(AbpTenantManagementApplicationContractsModule),
    typeof(AbpObjectExtendingModule)
)]
public class SharpSampleApplicationContractsModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        SharpSampleDtoExtensions.Configure();
    }
}
