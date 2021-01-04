using Volo.Abp.Modularity;

namespace SharpAbp.Abp.MassTransit
{
    [DependsOn(
        typeof(AbpMassTransitModule)
        )]
    public class AbpMassTransitKafkaModule : AbpModule
    {

    }
}
