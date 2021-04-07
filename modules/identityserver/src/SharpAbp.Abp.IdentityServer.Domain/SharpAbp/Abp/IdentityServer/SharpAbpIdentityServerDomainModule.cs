using Volo.Abp.IdentityServer;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.IdentityServer
{
    [DependsOn(
        typeof(AbpIdentityServerDomainModule)
        )]
    public class SharpAbpIdentityServerDomainModule : AbpModule
    {

    }
}
