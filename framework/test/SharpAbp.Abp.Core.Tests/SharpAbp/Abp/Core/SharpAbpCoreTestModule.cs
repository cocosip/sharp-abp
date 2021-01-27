using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Core
{
    [DependsOn(
      typeof(SharpAbpCoreModule),
      typeof(AbpTestBaseModule),
      typeof(AbpAutofacModule)
      )]
    public class SharpAbpCoreTestModule : AbpModule
    {

    }
}
