using SharpAbp.Abp.MassTransit;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;
using Volo.Abp.Timing;

namespace MassTransitSample.Common
{
    [DependsOn(
        typeof(AbpThreadingModule),
        typeof(AbpTimingModule),
        typeof(AbpMassTransitModule)
     )]
    public class MassTransitSampleCommonModule : AbpModule
    {

    }
}
