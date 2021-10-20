using SharpAbp.Abp.Identity;
using SharpAbp.Abp.IdentityServer;
using Volo.Abp.Account;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;

namespace IdentitySample
{
    [DependsOn(
        typeof(IdentitySampleDomainSharedModule),
        typeof(AbpAccountApplicationContractsModule),
        typeof(AbpFeatureManagementApplicationContractsModule),
        typeof(AbpPermissionManagementApplicationContractsModule),
        typeof(AbpSettingManagementApplicationContractsModule),
        typeof(AbpTenantManagementApplicationContractsModule),
        typeof(AbpObjectExtendingModule),
        typeof(AbpIdentityApplicationContractsModule),
        //typeof(AbpIdentityServerApplicationContractsModule),
        typeof(IdentityApplicationContractsModule),
        typeof(IdentityServerApplicationContractsModule)
    )]
    public class IdentitySampleApplicationContractsModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            IdentitySampleDtoExtensions.Configure();
        }
    }
}
