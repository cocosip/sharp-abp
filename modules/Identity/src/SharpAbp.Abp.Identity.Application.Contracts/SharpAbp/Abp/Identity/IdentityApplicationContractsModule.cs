using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.Identity
{
    [DependsOn(
        typeof(AbpValidationModule),
        typeof(AbpIdentityDomainSharedModule)
        )]
    public class IdentityApplicationContractsModule : AbpModule
    {

    }
}
