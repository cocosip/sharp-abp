using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.AspNetCore.Mvc
{
    [DependsOn(
        typeof(AbpAspNetCoreMvcModule),
        typeof(SharpAbpAspNetCoreModule)
        )]
    public class SharpAbpAspNetCoreMvcModule : AbpModule
    {

    }
}
