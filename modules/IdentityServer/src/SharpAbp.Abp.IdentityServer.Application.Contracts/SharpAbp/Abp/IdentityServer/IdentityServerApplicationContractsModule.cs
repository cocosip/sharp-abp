using SharpAbp.Abp.Identity;
using Volo.Abp.IdentityServer;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.IdentityServer
{
    [DependsOn(
        typeof(AbpValidationModule),
        typeof(AbpIdentityServerDomainSharedModule),
        typeof(IdentityApplicationContractsModule)
        )]
    public class IdentityServerApplicationContractsModule : AbpModule
    {

    }
}
