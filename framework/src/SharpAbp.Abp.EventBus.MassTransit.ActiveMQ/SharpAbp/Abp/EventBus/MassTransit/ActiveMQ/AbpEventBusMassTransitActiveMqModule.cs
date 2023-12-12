using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.MassTransit;
using SharpAbp.Abp.MassTransit.ActiveMQ;
using System;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.EventBus.MassTransit.ActiveMQ
{
    [DependsOn(
        typeof(AbpEventBusMassTransitModule),
        typeof(AbpMassTransitActiveMqModule)
        )]
    public class AbpEventBusMassTransitActiveMqModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            var abpMassTransitOptions = context.Services.ExecutePreConfiguredActions<AbpMassTransitOptions>();

            if (abpMassTransitOptions.Provider.Equals(MassTransitActiveMqConsts.ProviderName, StringComparison.OrdinalIgnoreCase))
            {
                var abpMassTransitEventBusOptions = context.Services.ExecutePreConfiguredActions<AbpMassTransitEventBusOptions>();

                //Producer
                PreConfigure<AbpMassTransitActiveMqOptions>(options =>
                {
                    options.Producers.Add(new ActiveMqProducerConfiguration()
                    {
                        QueueName = abpMassTransitEventBusOptions.Topic,
                        MessageConfigure = new Action<string, IActiveMqBusFactoryConfigurator>((exchangeName, c) =>
                        {
                            c.Message<AbpMassTransitEventData>(e =>
                            {
                                e.SetEntityName(exchangeName);
                            });
                        }),
                        MapConfigure = new Action<Uri>(u =>
                        {
                            EndpointConvention.Map<AbpMassTransitEventData>(u);
                        }),

                        PublishConfigure = new Action<Action<IActiveMqMessagePublishTopologyConfigurator>, IBusRegistrationContext, IActiveMqBusFactoryConfigurator>((preConfigure, ctx, cfg) =>
                        {
                            cfg.Publish<AbpMassTransitEventData>(c =>
                            {
                                preConfigure?.Invoke(c);
                            });
                        })
                    });

                    //Consumer
                    options.Consumers.Add(new ActiveMqConsumerConfiguration()
                    {
                        QueueName = abpMassTransitEventBusOptions.Topic,
                        Configure = new Action<IBusRegistrationConfigurator>(c =>
                        {
                            c.AddConsumer<MassTransitEventBusConsumer>();
                        }),
                        MessageConfigure = new Action<string, IActiveMqBusFactoryConfigurator>((exchangeName, c) =>
                        {
                            c.Message<AbpMassTransitEventData>(e =>
                            {
                                e.SetEntityName(exchangeName);
                            });
                        }),
                        ReceiveEndpoint = new Action<string, Action<string, IActiveMqReceiveEndpointConfigurator>, IBusRegistrationContext, IActiveMqBusFactoryConfigurator>((queueName, preConfigure, ctx, cfg) =>
                        {
                            cfg.ReceiveEndpoint(queueName, e =>
                            {
                                preConfigure?.Invoke(queueName, e);
                                e.Consumer<MassTransitEventBusConsumer>(ctx);
                            });
                        }),
                        MapConfigure = new Action<Uri>(u =>
                        {
                            EndpointConvention.Map<AbpMassTransitEventData>(u);
                        })
                    });
                });
            }

            return Task.CompletedTask;
        }
    }
}
