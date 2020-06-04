using DotCommon.ProtoBuf;
using Volo.Abp.Modularity;

namespace SharpAbp.DotCommon.ProtoBuf
{
    [DependsOn(typeof(SharpAbpDotCommonModule))]
    public class SharpAbpDotCommonProtoBufModule : AbpModule
    {

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddProtoBuf();
        }
    }
}
