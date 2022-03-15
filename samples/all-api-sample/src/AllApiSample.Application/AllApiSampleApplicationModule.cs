using SharpAbp.Abp.Account;
using SharpAbp.Abp.DbConnectionsManagement;
using SharpAbp.Abp.FileStoringManagement;
using SharpAbp.Abp.Identity;
using SharpAbp.Abp.IdentityServer;
using SharpAbp.Abp.MapTenancyManagement;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;

namespace AllApiSample;

[DependsOn(
    typeof(AllApiSampleDomainModule),
    typeof(AccountApplicationModule),
    typeof(AllApiSampleApplicationContractsModule),
    typeof(IdentityApplicationModule),
    typeof(IdentityServerApplicationModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(MapTenancyManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule),
    typeof(DbConnectionsManagementApplicationModule),
    typeof(FileStoringManagementApplicationModule)
    )]
public class AllApiSampleApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<AllApiSampleApplicationModule>();
        });
    }
}
