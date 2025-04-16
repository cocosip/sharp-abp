using SharpAbp.Abp.TenancyGrouping;
using Volo.Abp.AspNetCore;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.AspNetCore.TenancyGrouping
{
    [DependsOn(
        typeof(AbpTenancyGroupingModule),
        typeof(AbpAspNetCoreModule)
        )]
    public class AbpAspNetCoreTenancyGroupingModule : AbpModule
    {
    }
}
