using FastDFSCore;
using FastDFSCore.Transport;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.FastDFS
{
    public class AbpFastDFSModule : AbpModule
    {

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddFastDFSCore();
        }

        public override void OnApplicationShutdown(ApplicationShutdownContext context)
        {
           
        }
    }
}
