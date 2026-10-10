using SharpAbp.Abp.AspNetCore;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.AspNetCore.IpAccessControl
{
    [DependsOn(typeof(SharpAbpAspNetCoreModule))]
    public class SharpAbpAspNetCoreIpAccessControlModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<AbpIpAccessControlOptions>(options => { });
            return Task.CompletedTask;
        }
    }
}
