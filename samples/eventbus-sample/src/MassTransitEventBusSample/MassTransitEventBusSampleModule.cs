using SharpAbp.Abp.EventBus.MassTransit;
using SharpAbp.Abp.EventBus.MassTransit.ActiveMQ;
using SharpAbp.Abp.EventBus.MassTransit.Kafka;
using SharpAbp.Abp.EventBus.MassTransit.RabbitMQ;
using System.Threading.Tasks;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;
using Volo.Abp.Timing;

namespace MassTransitEventBusSample;

[DependsOn(
    typeof(AbpThreadingModule),
    typeof(AbpTimingModule),
    typeof(AbpEventBusMassTransitRabbitMQModule),
    typeof(AbpEventBusMassTransitKafkaModule),
    typeof(AbpEventBusMassTransitActiveMqModule),
    typeof(AbpAutofacModule)
)]
public class MassTransitEventBusSampleModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        AsyncHelper.RunSync(() => PreConfigureServicesAsync(context));
    }

    public override Task PreConfigureServicesAsync(ServiceConfigurationContext context)
    {
        PreConfigure<AbpMassTransitEventBusOptions>(options =>
        {
            options.Topic = "TestEventBus";
        });

        return Task.CompletedTask;
    }

}
