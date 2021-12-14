using Volo.Abp.AspNetCore;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.AspNetCore
{
    [DependsOn(
        typeof(AbpAspNetCoreModule)
        )]
    public class SharpAbpAspNetCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            
        }

    }
}
