using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.MassTransit;
using SharpAbp.Abp.MassTransit.Kafka;
using System;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.EventBus.MassTransit.Kafka
{
    [DependsOn(
       typeof(AbpEventBusMassTransitModule),
       typeof(AbpMassTransitKafkaModule)
       )]
    public class AbpEventBusMassTransitKafkaModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            var abpMassTransitOptions = context.Services.ExecutePreConfiguredActions<AbpMassTransitOptions>();

            if (abpMassTransitOptions.Provider!.Equals(MassTransitKafkaConsts.ProviderName, StringComparison.OrdinalIgnoreCase))
            {
                var abpMassTransitEventBusOptions = context.Services.ExecutePreConfiguredActions<AbpMassTransitEventBusOptions>();


                PreConfigure<AbpMassTransitKafkaOptions>(options =>
                {
                    //Producer
                    options.Producers.Add(new KafkaProducerConfiguration()
                    {
                        Topic = abpMassTransitEventBusOptions.Topic,
                        Configure = new Action<string, IRiderRegistrationConfigurator>((topic, c) =>
                        {
                            c.AddProducer<string, AbpMassTransitEventData>(topic);
                        })
                    });

                    //Consumer
                    options.Consumers.Add(new KafkaConsumerConfiguration()
                    {
                        Topic = abpMassTransitEventBusOptions.Topic,
                        Configure = new Action<IRiderRegistrationConfigurator>(c =>
                        {
                            c.AddConsumer<MassTransitEventBusConsumer>();
                        }),
                        TopicEndpointConfigure = new Action<string, string, Action<IKafkaTopicReceiveEndpointConfigurator>, IRiderRegistrationContext, IKafkaFactoryConfigurator>((topic, groupId, preConfigure, ctx, cfg) =>
                        {
                            cfg.TopicEndpoint<string, AbpMassTransitEventData>(topic, groupId, e =>
                            {
                                preConfigure?.Invoke(e);
                                e.ConfigureConsumer<MassTransitEventBusConsumer>(ctx);
                            });

                        })
                    });
                });

            }

            return Task.CompletedTask;
        }

    }
}
