using SharpAbp.Abp.Account;
using SharpAbp.Abp.DbConnectionsManagement;
using SharpAbp.Abp.FileStoringManagement;
using SharpAbp.Abp.Identity;
using SharpAbp.Abp.MapTenancyManagement;
using SharpAbp.Abp.OpenIddict;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;

namespace SharpSample;

[DependsOn(
    typeof(SharpSampleDomainModule),
    typeof(AccountApplicationModule),
    typeof(SharpSampleApplicationContractsModule),
    typeof(IdentityApplicationModule),
    typeof(OpenIddictApplicationModule),
    typeof(MapTenancyManagementApplicationModule),
    typeof(DbConnectionsManagementApplicationModule),
    typeof(FileStoringManagementApplicationModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule)
    )]
public class SharpSampleApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<SharpSampleApplicationModule>();
        });
    }
}
