using SharpAbp.Abp.FileStoringManagement;
using SharpAbp.Abp.MapTenancyManagement;
using Volo.Abp.Account;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.PermissionManagement;
using Volo.Abp.TenantManagement;

namespace SharpAbp.WebSample
{
    [DependsOn(
        typeof(WebSampleDomainSharedModule),
        typeof(AbpAccountApplicationContractsModule),
        typeof(AbpFeatureManagementApplicationContractsModule),
        typeof(AbpIdentityApplicationContractsModule),
        typeof(AbpPermissionManagementApplicationContractsModule),
        typeof(AbpTenantManagementApplicationContractsModule),
        typeof(FileStoringManagementApplicationContractsModule),
        typeof(MapTenancyManagementApplicationContractsModule),
        typeof(AbpObjectExtendingModule)
    )]
    public class WebSampleApplicationContractsModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            WebSampleDtoExtensions.Configure();
        }
    }
}
