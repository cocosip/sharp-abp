using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.MassTransit;
using SharpAbp.Abp.MassTransit.RabbitMQ;
using System;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.EventBus.MassTransit.RabbitMQ
{
    [DependsOn(
        typeof(AbpEventBusMassTransitModule),
        typeof(AbpMassTransitRabbitMqModule)
        )]
    public class AbpEventBusMassTransitRabbitMQModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            var abpMassTransitOptions = context.Services.ExecutePreConfiguredActions<AbpMassTransitOptions>();

            if (abpMassTransitOptions.Provider.Equals(MassTransitRabbitMqConsts.ProviderName, StringComparison.OrdinalIgnoreCase))
            {
                var abpMassTransitEventBusOptions = context.Services.ExecutePreConfiguredActions<AbpMassTransitEventBusOptions>();

                PreConfigure<AbpMassTransitRabbitMqOptions>(options =>
                {
                    //Producer
                    options.Producers.Add(new RabbitMqProducerConfiguration()
                    {
                        ExchangeName = abpMassTransitEventBusOptions.Topic,
                        MessageConfigure = new Action<string, IRabbitMqBusFactoryConfigurator>((exchangeName, c) =>
                        {
                            c.Message<AbpMassTransitEventData>(e =>
                            {
                                e.SetEntityName(exchangeName);
                            });
                        }),
                        PublishConfigure = new Action<Action<IRabbitMqMessagePublishTopologyConfigurator>, IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>((preConfigure, ctx, cfg) =>
                        {
                            cfg.Publish<AbpMassTransitEventData>(c =>
                            {
                                preConfigure?.Invoke(c);
                            });
                        })
                    });

                    //Consumer
                    options.Consumers.Add(new RabbitMqConsumerConfiguration()
                    {
                        ExchangeName = abpMassTransitEventBusOptions.Topic,
                        QueueName = abpMassTransitEventBusOptions.Topic,
                        Configure = new Action<IBusRegistrationConfigurator>(c =>
                        {
                            c.AddConsumer<MassTransitEventBusConsumer>();
                        }),
                        MessageConfigure = new Action<string, IRabbitMqBusFactoryConfigurator>((exchangeName, c) =>
                        {
                            c.Message<MassTransitEventBusConsumer>(e =>
                            {
                                e.SetEntityName(exchangeName);
                            });
                        }),
                        ReceiveEndpoint = new Action<string, string, Action<string, string, IRabbitMqReceiveEndpointConfigurator>, IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>((exchangeName, queueName, preConfigure, ctx, cfg) =>
                        {
                            cfg.ReceiveEndpoint(queueName, e =>
                            {
                                preConfigure?.Invoke(exchangeName, queueName, e);
                                e.Consumer<MassTransitEventBusConsumer>(ctx);
                            });
                        })
                    });
                });

            }

            return Task.CompletedTask;
        }

    }
}
