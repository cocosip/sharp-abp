using FastDFSCore;
using Volo.Abp.Modularity;

namespace SharpAbp.FastDFSCore.Transport.DotNetty
{
    [DependsOn(typeof(SharpAbpFastDFSCoreModule))]
    public class SharpAbpFastDFSCoreTransportDotNettyModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddFastDFSDotNettyTransport();
        }

    }
}
