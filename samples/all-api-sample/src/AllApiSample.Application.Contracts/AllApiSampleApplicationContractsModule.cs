using SharpAbp.Abp.Account;
using SharpAbp.Abp.DbConnectionsManagement;
using SharpAbp.Abp.FileStoringManagement;
using SharpAbp.Abp.Identity;
using SharpAbp.Abp.IdentityServer;
using SharpAbp.Abp.MapTenancyManagement;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;

namespace AllApiSample;

[DependsOn(
    typeof(AllApiSampleDomainSharedModule),
    typeof(AccountApplicationContractsModule),
    typeof(AbpFeatureManagementApplicationContractsModule),
    typeof(IdentityApplicationContractsModule),
    typeof(IdentityServerApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationContractsModule),
    typeof(AbpSettingManagementApplicationContractsModule),
    typeof(MapTenancyManagementApplicationContractsModule),
    typeof(DbConnectionsManagementApplicationContractsModule),
    typeof(FileStoringManagementApplicationContractsModule),
    typeof(AbpObjectExtendingModule)
)]
public class AllApiSampleApplicationContractsModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        AllApiSampleDtoExtensions.Configure();
    }
}
