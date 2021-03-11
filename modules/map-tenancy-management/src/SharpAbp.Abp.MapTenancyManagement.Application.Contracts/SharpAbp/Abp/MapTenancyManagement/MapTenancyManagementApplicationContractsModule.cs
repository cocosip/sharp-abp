using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.MapTenancyManagement
{
    [DependsOn(
        typeof(MapTenancyManagementDomainSharedModule),
        typeof(AbpDddApplicationContractsModule)
        )]
    public class MapTenancyManagementApplicationContractsModule : AbpModule
    {

    }
}
