using SharpAbp.Abp.Binary.Protobuf;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Binary
{
    [DependsOn(
        typeof(AbpBinaryProtobufModule)
        )]
    public class AbpBinaryModule : AbpModule
    {
    }
}
