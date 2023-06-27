using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Binary
{
    [DependsOn(
        typeof(AbpBinaryModule),
        typeof(AbpAutofacModule),
        typeof(AbpTestBaseModule)
        )]
    public class AbpBinaryTestModule : AbpModule
    {

    }
}
