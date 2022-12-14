using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict.Applications;

namespace SharpAbp.Abp.OpenIddict
{
    [DependsOn(
        typeof(AbpDddApplicationModule),
        typeof(AbpAutoMapperModule),
        typeof(OpenIddictApplicationContractsModule),
        typeof(OpenIddictDomainModule)
        )]
    public class OpenIddictApplicationModule : AbpModule
    {

    }
}
