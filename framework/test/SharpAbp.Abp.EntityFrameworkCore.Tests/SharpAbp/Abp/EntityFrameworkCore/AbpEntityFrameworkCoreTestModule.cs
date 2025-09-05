using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.EntityFrameworkCore
{
    [DependsOn(
        typeof(SharpAbpEntityFrameworkCoreModule),
        typeof(AbpAutofacModule),
        typeof(AbpTestBaseModule)
       )]
    public class AbpEntityFrameworkCoreTestModule : AbpModule
    {
    }
}