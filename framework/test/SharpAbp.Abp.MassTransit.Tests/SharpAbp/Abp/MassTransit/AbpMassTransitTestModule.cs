using SharpAbp.Abp.MassTransit.Kafka;
using SharpAbp.Abp.MassTransit.RabbitMQ;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.MassTransit
{
    [DependsOn(
        typeof(AbpMassTransitKafkaModule),
        typeof(AbpMassTransitRabbitMqModule),
        typeof(AbpTestBaseModule),
        typeof(AbpAutofacModule)
      )]
    public class AbpMassTransitTestModule: AbpModule
    {

    }
}
