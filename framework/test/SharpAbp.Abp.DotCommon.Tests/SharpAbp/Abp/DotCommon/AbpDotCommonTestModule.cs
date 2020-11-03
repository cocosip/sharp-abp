using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.DotCommon
{
    [DependsOn(
      typeof(AbpDotCommonModule),
      typeof(AbpTestBaseModule),
      typeof(AbpAutofacModule)
      )]
    public class AbpDotCommonTestModule : AbpModule
    {

    }
}
