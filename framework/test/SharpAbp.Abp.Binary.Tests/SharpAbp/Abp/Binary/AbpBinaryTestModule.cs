using SharpAbp.Abp.Binary.Protobuf;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Binary
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(AbpBinaryProtobufModule),
        typeof(AbpTestBaseModule)
        )]
    public class AbpBinaryTestModule : AbpModule
    {

    }
}
