using FastDFSCore;
using Volo.Abp.Modularity;


namespace SharpAbp.Abp.FastDFS.DotNetty
{
    [DependsOn(typeof(AbpFastDFSModule))]
    public class AbpFastDFSDotNettyModule : AbpModule
    {

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddFastDFSDotNetty();
        }
    }
}
