using Volo.Abp.AspNetCore;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.TransformSecurity.AspNetCore
{
    [DependsOn(
        typeof(AbpTransformSecurityModule),
        typeof(AbpAspNetCoreModule)
        )]
    public class AbpTransformSecurityAspNetCoreModule : AbpModule
    {

    }
}
