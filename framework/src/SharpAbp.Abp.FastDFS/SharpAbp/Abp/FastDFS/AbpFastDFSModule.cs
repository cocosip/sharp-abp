using FastDFSCore;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.FastDFS
{
    public class AbpFastDFSModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddFastDFSCore();
        }
    }
}
