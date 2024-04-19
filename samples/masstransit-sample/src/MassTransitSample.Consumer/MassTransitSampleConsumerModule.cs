using MassTransit;
using MassTransitSample.Common;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.MassTransit;
using SharpAbp.Abp.MassTransit.ActiveMQ;
using SharpAbp.Abp.MassTransit.Kafka;
using SharpAbp.Abp.MassTransit.PostgreSql;
using SharpAbp.Abp.MassTransit.RabbitMQ;
using System;
using System.Threading.Tasks;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace MassTransitSample.Consumer
{
    [DependsOn(
        typeof(MassTransitSampleCommonModule),
        typeof(AbpMassTransitRabbitMqModule),
        typeof(AbpMassTransitKafkaModule),
        typeof(AbpMassTransitActiveMqModule),
        typeof(AbpMassTransitPostgreSqlModule),
        typeof(AbpAutofacModule)
        )]
    public class MassTransitSampleConsumerModule : AbpModule
    {

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            var abpMassTransitOptions = context.Services.ExecutePreConfiguredActions<AbpMassTransitOptions>();

            if (abpMassTransitOptions.Provider.Equals(MassTransitRabbitMqConsts.ProviderName, StringComparison.OrdinalIgnoreCase))
            {
                PreConfigure<AbpMassTransitRabbitMqOptions>(options =>
                {
                    options.Consumers.Add(new RabbitMqConsumerConfiguration()
                    {
                        ExchangeName = RabbitMqQueues.Exchange1,
                        QueueName = RabbitMqQueues.Queue1,
                        Configure = new Action<IBusRegistrationConfigurator>(c =>
                        {
                            c.AddConsumer<MassTransitSampleConsumer>();
                        }),
                        MessageConfigure = new Action<string, IRabbitMqBusFactoryConfigurator>((exchangeName, c) =>
                        {
                            c.Message<MassTransitSampleMessage>(e =>
                            {
                                e.SetEntityName(exchangeName);
                            });
                        }),
                        ReceiveEndpoint = new Action<string, string, Action<string, string, IRabbitMqReceiveEndpointConfigurator>, IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>((exchangeName, queueName, preConfigure, ctx, cfg) =>
                        {
                            cfg.ReceiveEndpoint(queueName, e =>
                            {
                                preConfigure?.Invoke(exchangeName, queueName, e);
                                e.ConfigureConsumer<MassTransitSampleConsumer>(ctx);
                            });

                        })

                    });
                });
            }
            else if (abpMassTransitOptions.Provider.Equals(MassTransitKafkaConsts.ProviderName, StringComparison.OrdinalIgnoreCase))
            {
                PreConfigure<AbpMassTransitKafkaOptions>(options =>
                {
                    options.Consumers.Add(new KafkaConsumerConfiguration()
                    {
                        Topic = KafkaTopics.Topic1,
                        Configure = new Action<IRiderRegistrationConfigurator>(c =>
                        {
                            c.AddConsumer<MassTransitSampleConsumer>();
                        }),
                        TopicEndpointConfigure = new Action<string, string, Action<IKafkaTopicReceiveEndpointConfigurator>, IRiderRegistrationContext, IKafkaFactoryConfigurator>((topic, groupId, preConfigure, ctx, cfg) =>
                        {
                            cfg.TopicEndpoint<string, MassTransitSampleMessage>(topic, groupId, e =>
                            {
                                preConfigure?.Invoke(e);
                                e.ConfigureConsumer<MassTransitSampleConsumer>(ctx);
                            });

                        })
                    });
                });
            }
            else if (abpMassTransitOptions.Provider.Equals(MassTransitActiveMqConsts.ProviderName, StringComparison.OrdinalIgnoreCase))
            {
                PreConfigure<AbpMassTransitActiveMqOptions>(options =>
                {
                    options.Consumers.Add(new ActiveMqConsumerConfiguration()
                    {
                        QueueName = ActiveMqQueues.Queue1,
                        Configure = new Action<IBusRegistrationConfigurator>(c =>
                        {
                            c.AddConsumer<MassTransitSampleConsumer>();
                        }),


                        ReceiveEndpoint = new Action<string, Action<string, IActiveMqReceiveEndpointConfigurator>, IBusRegistrationContext, IActiveMqBusFactoryConfigurator>((queueName, preConfigure, ctx, cfg) =>
                        {
                            cfg.ReceiveEndpoint(queueName, e =>
                            {
                                preConfigure?.Invoke(queueName, e);
                                e.ConfigureConsumer<MassTransitSampleConsumer>(ctx);
                            });
                        }),
                        MapConfigure = new Action<Uri>(u =>
                        {
                            EndpointConvention.Map<MassTransitSampleMessage>(u);
                        })
                    });
                });
            }
            else if (abpMassTransitOptions.Provider.Equals(MassTransitPostgreSqlConsts.ProviderName, StringComparison.OrdinalIgnoreCase))
            {
                MessageCorrelation.UseCorrelationId<MassTransitSampleMessage>(x => Guid.Parse(x.MessageId));

                PreConfigure<AbpMassTransitPostgreSqlOptions>(options =>
                {
                    options.Consumers.Add(new PostgreSqlConsumerConfiguration()
                    {
                        Configure = new Action<IBusRegistrationConfigurator>(c =>
                        {
                            c.AddConsumer<MassTransitSampleConsumer>();
                        }),
                        Types = [typeof(MassTransitSampleMessage)]
                    });
                });
            }
            return Task.CompletedTask;
        }

    }
}
