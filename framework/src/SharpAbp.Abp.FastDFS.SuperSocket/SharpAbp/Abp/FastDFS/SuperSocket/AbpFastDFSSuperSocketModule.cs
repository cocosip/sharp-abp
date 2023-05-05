using FastDFSCore;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.FastDFS.SuperSocket
{
    [DependsOn(typeof(AbpFastDFSModule))]
    public class AbpFastDFSSuperSocketModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddFastDFSSuperSocket();
            return Task.CompletedTask;
        }
    }
}
