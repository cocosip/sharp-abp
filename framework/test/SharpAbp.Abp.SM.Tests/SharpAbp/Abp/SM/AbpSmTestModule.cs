using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.SM
{
    [DependsOn(
        typeof(AbpSmModule),
        typeof(AbpAutofacModule), 
        typeof(AbpTestBaseModule)
       )]
    public class AbpSmTestModule : AbpModule
    {

    }
}
