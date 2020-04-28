using DotCommon.ProtoBuf;
using DotCommon.Serializing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.Modularity;

namespace SharpAbp.DotCommon.ProtoBuf
{
    [DependsOn(typeof(SharpAbpDotCommonModule))]
    public class SharpAbpDotCommonProtoBufModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.Replace(ServiceDescriptor.Singleton<IBinarySerializer,ProtocolBufSerializer>());
        }
    }
}
