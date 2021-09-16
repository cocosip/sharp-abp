using Volo.Abp.IdentityServer;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.IdentityServer
{
    [DependsOn(
        typeof(AbpValidationModule),
        typeof(AbpIdentityServerDomainSharedModule)
        )]
    public class IdentityServerApplicationContractsModule : AbpModule
    {

    }
}
