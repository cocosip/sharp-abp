using SharpAbp.Abp.MassTransit;
using Volo.Abp.EventBus;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.EventBus.MassTransit
{
    [DependsOn(
        typeof(AbpEventBusModule),
        typeof(AbpMassTransitModule)
        )]
    public class AbpEventBusMassTransitModule : AbpModule
    {

    }
}
