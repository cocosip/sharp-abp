using FastDFSCore;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.FastDFS.DotNetty
{
    [DependsOn(typeof(AbpFastDFSModule))]
    public class AbpFastDFSDotNettyModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddFastDFSDotNetty();
            return Task.CompletedTask;
        }
    }
}
