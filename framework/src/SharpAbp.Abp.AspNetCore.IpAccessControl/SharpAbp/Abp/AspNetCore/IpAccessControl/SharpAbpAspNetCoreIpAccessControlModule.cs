using SharpAbp.Abp.AspNetCore;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.AspNetCore.IpAccessControl
{
    [DependsOn(typeof(SharpAbpAspNetCoreModule))]
    public class SharpAbpAspNetCoreIpAccessControlModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpIpAccessControlOptions>(options => { });
        }
    }
}
