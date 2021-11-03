using Volo.Abp.Modularity;

namespace SharpAbp.Abp.IdentityServer.Extensions
{
    [DependsOn(
        typeof(IdentityServerDomainModule)
        )]
    public class IdentityServerExtensionsModule : AbpModule
    {

    }
}
