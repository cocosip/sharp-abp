using DotCommon.Json4Net;
using Volo.Abp.Modularity;

namespace SharpAbp.DotCommon.Json4Net
{
    [DependsOn(typeof(SharpAbpDotCommonModule))]
    public class SharpAbpDotCommonJson4NetModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddJson4Net();
        }

    }
}
