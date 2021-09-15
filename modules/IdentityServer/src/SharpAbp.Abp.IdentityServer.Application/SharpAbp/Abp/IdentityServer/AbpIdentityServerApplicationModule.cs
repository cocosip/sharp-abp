using Volo.Abp.IdentityServer;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.IdentityServer
{
    [DependsOn(
        typeof(AbpIdentityServerApplicationContractsModule),
        typeof(AbpIdentityServerDomainModule)
        )]
    public class AbpIdentityServerApplicationModule : AbpModule
    {

    }
}
