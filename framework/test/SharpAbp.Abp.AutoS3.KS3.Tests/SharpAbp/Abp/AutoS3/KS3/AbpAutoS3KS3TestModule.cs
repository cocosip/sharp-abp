using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.AutoS3.KS3
{
    [DependsOn(
      typeof(AbpAutoS3KS3Module),
      typeof(AbpTestBaseModule),
      typeof(AbpAutofacModule)
      )]
    public class AbpAutoS3KS3TestModule : AbpModule
    {

    }
}
