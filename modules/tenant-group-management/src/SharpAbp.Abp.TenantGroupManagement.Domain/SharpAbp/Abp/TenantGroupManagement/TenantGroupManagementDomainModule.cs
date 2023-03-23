using SharpAbp.Abp.TenancyGrouping;
using Volo.Abp.Caching;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement;

namespace SharpAbp.Abp.TenantGroupManagement
{
    [DependsOn(
        typeof(AbpCachingModule),
        typeof(AbpTenancyGroupingModule),
        typeof(AbpTenantManagementDomainModule),
        typeof(TenantGroupManagementDomainSharedModule)
        )]
    public class TenantGroupManagementDomainModule : AbpModule
    {

    }
}
