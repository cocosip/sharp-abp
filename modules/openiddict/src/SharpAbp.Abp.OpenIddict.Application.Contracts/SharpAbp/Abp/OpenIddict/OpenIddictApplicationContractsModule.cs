using Volo.Abp.Application;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.OpenIddict
{
    [DependsOn(
        typeof(AbpValidationModule),
        typeof(OpenIddictDomainSharedModule),
        typeof(AbpDddApplicationContractsModule)
        )]
    public class OpenIddictApplicationContractsModule : AbpModule
    {

    }
}
