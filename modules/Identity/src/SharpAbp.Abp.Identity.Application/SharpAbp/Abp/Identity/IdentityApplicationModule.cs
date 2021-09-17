using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Identity
{
    [DependsOn(
        typeof(AbpDddApplicationModule),
        typeof(AbpAutoMapperModule),
        typeof(IdentityApplicationContractsModule),
        typeof(IdentityDomainModule)
        )]
    public class IdentityApplicationModule : AbpModule
    {

    }
}
