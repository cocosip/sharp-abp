using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.MassTransit.Kafka
{
    [DependsOn(
        typeof(AbpMassTransitModule)
        )]
    public class AbpMassTransitKafkaModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            PreConfigure<AbpMassTransitOptions>(options =>
            {
                options.PreConfigures.Add(new Action<IServiceCollectionBusConfigurator>(c =>
                {
                    c.UsingInMemory();
                }));
            });

            PreConfigure<AbpMassTransitKafkaOptions>(options =>
            {
                options.DefaultTopicFormatFunc = KafkaUtil.TopicFormat;
                options.DefaultConsumerGroupId = "SharpAbp.MassTransit";
            });

        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var massTransitOptions = context.Services.ExecutePreConfiguredActions<AbpMassTransitOptions>();

            var kafkaOptions = context.Services.ExecutePreConfiguredActions<AbpMassTransitKafkaOptions>();

            context.Services.AddMassTransit(x =>
            {
                //PreConfigure
                foreach (var preConfigure in massTransitOptions.PreConfigures)
                {
                    preConfigure(x);
                }

                x.AddRider(rider =>
                {
                    //Producer
                    foreach (var producer in kafkaOptions.Producers)
                    {
                        var topic = kafkaOptions.DefaultTopicFormatFunc(massTransitOptions.Prefix, producer.Topic);
                        producer.Configure(topic, rider);
                    }

                    //Consumer
                    foreach (var consumer in kafkaOptions.Consumers)
                    {
                        consumer.Configure(rider);
                    }

                    rider.UsingKafka((ctx, k) =>
                    {
                        k.Host(kafkaOptions.Server, c =>
                        {
                            if (kafkaOptions.UseSsl && kafkaOptions.ConfigureSsl != null)
                            {
                                c.UseSsl(kafkaOptions.ConfigureSsl);
                            }
                        });

                        foreach (var consumer in kafkaOptions.Consumers)
                        {
                            var topic = kafkaOptions.DefaultTopicFormatFunc(massTransitOptions.Prefix, consumer.Topic);

                            var group = consumer.GroupId.IsNullOrWhiteSpace() ? kafkaOptions.DefaultConsumerGroupId : consumer.GroupId;

                            consumer.TopicEndpointConfigure(topic, group, consumer.ReceiveEndpointConfigure, ctx, k);
                        }

                    });
                });

                //PostConfigure
                foreach (var postConfigure in massTransitOptions.PostConfigures)
                {
                    postConfigure(x);
                }
            });
        }
    }
}
