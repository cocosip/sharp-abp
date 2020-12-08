using FastDFSCore;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.FastDFS.SuperSocket
{
    [DependsOn(
        typeof(AbpFastDFSModule)
        )]
    public class AbpFastDFSSuperSocketModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddFastDFSSuperSocket();
        }
    }
}
