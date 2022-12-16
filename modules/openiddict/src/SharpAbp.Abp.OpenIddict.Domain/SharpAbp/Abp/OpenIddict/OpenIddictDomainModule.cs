using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict;

namespace SharpAbp.Abp.OpenIddict
{
    [DependsOn(
        typeof(AbpOpenIddictDomainModule),
        typeof(OpenIddictDomainSharedModule)
        )]
    public class OpenIddictDomainModule : AbpModule
    {

    }
}
