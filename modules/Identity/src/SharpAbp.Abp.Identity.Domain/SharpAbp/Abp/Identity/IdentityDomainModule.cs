using Volo.Abp.Identity;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Identity
{
    [DependsOn(
        typeof(AbpIdentityDomainModule),
        typeof(IdentityDomainSharedModule)
        )]
    public class IdentityDomainModule : AbpModule
    {

    }
}
