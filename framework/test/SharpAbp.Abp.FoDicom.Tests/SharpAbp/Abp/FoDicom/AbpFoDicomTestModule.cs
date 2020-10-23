using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.FoDicom
{
    [DependsOn(
        typeof(AbpFoDicomModule),
        typeof(AbpTestBaseModule),
        typeof(AbpAutofacModule)
    )]
    public class AbpFoDicomTestModule : AbpModule
    {


    }
}
