using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement;

namespace SharpAbp.Abp.TenantGroupManagement
{
    [DependsOn(
        typeof(TenantGroupManagementDomainSharedModule),
        typeof(AbpTenantManagementApplicationContractsModule),
        typeof(AbpDddApplicationContractsModule),
        typeof(AbpAuthorizationModule)
        )]
    public class TenantGroupManagementApplicationContractsModule : AbpModule
    {

    }
}
