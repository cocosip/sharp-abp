using Volo.Abp.IdentityServer;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.IdentityServer
{
    [DependsOn(
        typeof(AbpIdentityServerDomainSharedModule)
        )]
    public class AbpIdentityServerApplicationContractsModule : AbpModule
    {

    }
}
