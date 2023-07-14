using MassTransit;
using MassTransitSample.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.MassTransit;
using SharpAbp.Abp.MassTransit.ActiveMQ;
using SharpAbp.Abp.MassTransit.Kafka;
using SharpAbp.Abp.MassTransit.RabbitMQ;
using System;
using System.Threading.Tasks;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace MassTransitSample.Producer
{
    [DependsOn(
        typeof(MassTransitSampleCommonModule),
        typeof(AbpMassTransitRabbitMqModule),
        typeof(AbpMassTransitKafkaModule),
        typeof(AbpMassTransitActiveMqModule),
        typeof(AbpAutofacModule)
        )]
    public class MassTransitSampleProducerModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => PreConfigureServicesAsync(context));
        }

        public override Task PreConfigureServicesAsync(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            var abpMassTransitOptions = configuration
                .GetSection("MassTransitOptions")
                .Get<AbpMassTransitOptions>();

            if (abpMassTransitOptions.Provider.Equals(MassTransitRabbitMqConsts.ProviderName, StringComparison.OrdinalIgnoreCase))
            {
                PreConfigure<AbpMassTransitRabbitMqOptions>(options =>
                {
                    options.Producers.Add(new RabbitMqProducerConfiguration()
                    {
                        ExchangeName = RabbitMqQueues.Exchange1,
                        MessageConfigure = new Action<string, IRabbitMqBusFactoryConfigurator>((exchangeName, c) =>
                        {
                            c.Message<MassTransitSampleMessage>(e =>
                            {
                                e.SetEntityName(exchangeName);
                            });

                        }),
                        PublishConfigure = new Action<Action<IRabbitMqMessagePublishTopologyConfigurator>, IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>((preConfigure, ctx, cfg) =>
                        {
                            cfg.Publish<MassTransitSampleMessage>(c =>
                            {
                                preConfigure?.Invoke(c);
                            });
                        })
                    });

                });
            }
            else if (abpMassTransitOptions.Provider.Equals(MassTransitKafkaConsts.ProviderName, StringComparison.OrdinalIgnoreCase))
            {
                PreConfigure<AbpMassTransitKafkaOptions>(options =>
                {
                    options.Producers.Add(new KafkaProducerConfiguration()
                    {
                        Topic = KafkaTopics.Topic1,
                        Configure = new Action<string, IRiderRegistrationConfigurator>((topic, c) =>
                        {
                            c.AddProducer<string, MassTransitSampleMessage>(topic);
                        })
                    });
                });
            }
            else if (abpMassTransitOptions.Provider.Equals(MassTransitActiveMqConsts.ProviderName, StringComparison.OrdinalIgnoreCase))
            {
                PreConfigure<AbpMassTransitActiveMqOptions>(options =>
                {
                    options.Producers.Add(new ActiveMqProducerConfiguration()
                    {
                        QueueName = ActiveMqQueues.Queue1,

                        PublishConfigure = new Action<Action<IActiveMqMessagePublishTopologyConfigurator>, IBusRegistrationContext, IActiveMqBusFactoryConfigurator>((p, ctx, cfg) =>
                        {
                            cfg.Publish<MassTransitSampleMessage>(c =>
                            {
                                p?.Invoke(c);
                            });
                        }),

                        MapConfigure = new Action<Uri>(u =>
                        {
                            EndpointConvention.Map<MassTransitSampleMessage>(u);
                        }),

                    });

                });
            }

            return Task.CompletedTask;
        }
    }
}
