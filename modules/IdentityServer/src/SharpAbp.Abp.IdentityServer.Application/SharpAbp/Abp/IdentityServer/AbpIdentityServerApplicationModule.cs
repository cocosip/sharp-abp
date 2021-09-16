using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.IdentityServer
{
    [DependsOn(
        typeof(AbpDddApplicationModule),
        typeof(AbpAutoMapperModule),
        typeof(IdentityServerApplicationContractsModule),
        typeof(IdentityServerDomainModule)
        )]
    public class AbpIdentityServerApplicationModule : AbpModule
    {

    }
}
