using FastDFSCore;
using Volo.Abp.Modularity;

namespace SharpAbp.FastDFSCore.Transport.SuperSocket
{
    [DependsOn(typeof(SharpAbpFastDFSCoreModule))]
    public class SharpAbpFastDFSCoreSuperSocketModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddFastDFSSuperSocket();
        }
    }
}
