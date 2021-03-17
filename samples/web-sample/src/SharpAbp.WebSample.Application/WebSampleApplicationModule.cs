using SharpAbp.Abp.FileStoringManagement;
using SharpAbp.Abp.MapTenancyManagement;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.TenantManagement;

namespace SharpAbp.WebSample
{
    [DependsOn(
        typeof(WebSampleDomainModule),
        typeof(AbpAccountApplicationModule),
        typeof(WebSampleApplicationContractsModule),
        typeof(AbpIdentityApplicationModule),
        typeof(AbpPermissionManagementApplicationModule),
        typeof(AbpTenantManagementApplicationModule),
        typeof(AbpFeatureManagementApplicationModule),
        typeof(FileStoringManagementApplicationModule),
        typeof(MapTenancyManagementApplicationModule)
        )]
    public class WebSampleApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<WebSampleApplicationModule>();
            });
        }
    }
}
