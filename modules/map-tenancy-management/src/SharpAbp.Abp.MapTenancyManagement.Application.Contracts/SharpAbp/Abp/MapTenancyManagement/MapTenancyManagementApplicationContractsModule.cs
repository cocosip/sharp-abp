using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.MapTenancyManagement
{
    [DependsOn(
        typeof(MapTenancyManagementDomainSharedModule),
        typeof(AbpDddApplicationContractsModule),
        typeof(AbpAuthorizationModule)
        )]
    public class MapTenancyManagementApplicationContractsModule : AbpModule
    {

    }
}
