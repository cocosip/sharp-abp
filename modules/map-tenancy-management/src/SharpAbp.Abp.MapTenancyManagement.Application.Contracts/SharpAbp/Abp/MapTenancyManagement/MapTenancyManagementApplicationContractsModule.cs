using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement;

namespace SharpAbp.Abp.MapTenancyManagement
{
    [DependsOn(
        typeof(MapTenancyManagementDomainSharedModule),
        typeof(AbpTenantManagementApplicationContractsModule),
        typeof(AbpDddApplicationContractsModule),
        typeof(AbpAuthorizationModule)
        )]
    public class MapTenancyManagementApplicationContractsModule : AbpModule
    {

    }
}
