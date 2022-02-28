using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.KafkaIntegration;
using MassTransit.Registration;
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

        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var massTransitOptions = context.Services.ExecutePreConfiguredActions<AbpMassTransitOptions>();

            var kafkaOptions = context.Services.ExecutePreConfiguredActions<AbpMassTransitKafkaOptions>();

            context.Services.AddMassTransit(x =>
            {
                //Preconfig
                foreach (var preConfigure in massTransitOptions.PreConfigures)
                {
                    preConfigure(x);
                }

                x.AddRider(rider =>
                {
                    //Producer
                    foreach (var producer in kafkaOptions.Producers)
                    {
                        var topic = "";
                        producer.Configurator(topic, rider);
                    }

                    //Consumer
                    foreach (var consumer in kafkaOptions.Consumers)
                    {
                        consumer.Configurator(rider);
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
                            var topic = "";
                            var group = "";

                            consumer.TopicEndpointConfigurator(topic, group, consumer.ReceiveEndpointConfigurator, ctx, k);
                        }

                    });
                });
            });



            var o = new AbpMassTransitKafkaOptions();
            o.Producers.Add(new KafkaProducerConfiguration()
            {
                Topic = "1",
                Configurator = new Action<string, IRiderRegistrationConfigurator>((topic, cfg) =>
                {
                    cfg.AddProducer<string, Class111>(topic);
                })
            });

            o.Consumers.Add(new KafkaConsumerConfiguration()
            {
                Topic = "consumer1",
                Configurator = new Action<IRiderRegistrationConfigurator>(cfg =>
                {
                    cfg.AddConsumer<Class111Consumer>();
                }),
                TopicEndpointConfigurator = new Action<string, string, Action<IKafkaTopicReceiveEndpointConfigurator>, IRiderRegistrationContext, IKafkaFactoryConfigurator>((topic, group, preConfigure, ctx, cfg) =>
                {
                    cfg.TopicEndpoint<string, Class111>(topic, group, e =>
                    {
                        preConfigure?.Invoke(e);
                        e.ConfigureConsumer<Class111Consumer>(ctx);
                    });
                })
            });

        }
    }

    public class Class111
    {

    }

    public class Class111Consumer : IConsumer
    {

    }
}
