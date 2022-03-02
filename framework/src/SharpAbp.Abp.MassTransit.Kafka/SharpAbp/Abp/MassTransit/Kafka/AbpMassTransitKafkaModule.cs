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

            PreConfigure<AbpMassTransitKafkaOptions>(options =>
            {
                options.DefaultTopicFormatFunc = KafkaUtil.TopicFormat;

                //Kafka keep alive
                options.KafkaConfigures.Add(new Action<IRiderRegistrationContext, IKafkaFactoryConfigurator>((ctx, k) =>
                {
                    k.ConfigureSocket(s =>
                    {
                        s.KeepaliveEnable = true;
                    });
                }));

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
                    //Rider preConfigure
                    foreach (var preConfigure in kafkaOptions.RiderPreConfigures)
                    {
                        preConfigure(rider);
                    }

                    //Producer
                    foreach (var producer in kafkaOptions.Producers)
                    {
                        var topic = kafkaOptions.DefaultTopicFormatFunc(massTransitOptions.Prefix, producer.Topic);
                        producer.Configure?.Invoke(topic, rider);
                    }

                    //Consumer
                    foreach (var consumer in kafkaOptions.Consumers)
                    {
                        consumer.Configure?.Invoke(rider);
                    }

                    //Rider configure
                    foreach (var configure in kafkaOptions.RiderConfigures)
                    {
                        configure(rider);
                    }

                    rider.UsingKafka((ctx, k) =>
                    {
                        //Kafka preConfigure
                        foreach (var preConfigure in kafkaOptions.KafkaPreConfigures)
                        {
                            preConfigure(ctx, k);
                        }

                        k.Host(kafkaOptions.Server, c =>
                        {
                            if (kafkaOptions.UseSsl && kafkaOptions.ConfigureSsl != null)
                            {
                                c.UseSsl(kafkaOptions.ConfigureSsl);
                            }
                        });

                        k.TopicEndpoint<string, MassTransitBus>("", "", c =>
                        {
                            c.MaxPollInterval = TimeSpan.FromSeconds(1);
                        });


                        //Kafka configures
                        foreach (var configure in kafkaOptions.KafkaConfigures)
                        {
                            configure(ctx, k);
                        }

                        foreach (var consumer in kafkaOptions.Consumers)
                        {
                            var topic = kafkaOptions.DefaultTopicFormatFunc(massTransitOptions.Prefix, consumer.Topic);

                            // var groupId = consumer.GroupId.IsNullOrWhiteSpace() ? kafkaOptions.DefaultGroupId : consumer.GroupId;
                            var groupId = "";

                            var receiveEndpointConfigure = consumer.ReceiveEndpointConfigure ?? kafkaOptions.DefaultReceiveEndpointConfigure;

                            consumer.TopicEndpointConfigure?.Invoke(topic, groupId, receiveEndpointConfigure, ctx, k);
                        }

                        //k.TopicEndpoint<string, AbpMassTransitKafkaModule>("", "", c =>
                        //{
                        //    c.CheckpointInterval
                        //});


                        //Kafka postConfigure
                        foreach (var postConfigure in kafkaOptions.KafkaPostConfigures)
                        {
                            postConfigure(ctx, k);
                        }

                    });

                    //Rider postConfigure
                    foreach (var postConfigure in kafkaOptions.RiderPostConfigures)
                    {
                        postConfigure(rider);
                    }

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
