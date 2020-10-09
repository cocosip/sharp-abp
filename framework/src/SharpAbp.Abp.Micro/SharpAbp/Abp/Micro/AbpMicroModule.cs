using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.Micro
{
    [DependsOn(typeof(AbpThreadingModule))]
    public class AbpMicroModule : AbpModule
    {

    }
}
