using SharpAbp.Abp.Account;
using SharpAbp.Abp.Identity;
using SharpAbp.Abp.IdentityServer;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;

namespace IdentitySample
{
    [DependsOn(
        typeof(IdentitySampleDomainModule),
        typeof(AbpAccountApplicationModule),
        typeof(AccountApplicationModule),
        typeof(IdentitySampleApplicationContractsModule),
        typeof(AbpIdentityApplicationModule),
        typeof(IdentityApplicationModule),
        typeof(IdentityServerApplicationModule),
        typeof(AbpPermissionManagementApplicationModule),
        typeof(AbpTenantManagementApplicationModule),
        typeof(AbpFeatureManagementApplicationModule),
        typeof(AbpSettingManagementApplicationModule)
        )]
    public class IdentitySampleApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<IdentitySampleApplicationModule>();
            });
        }
    }
}
