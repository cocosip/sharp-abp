using MassTransit;
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
                options.PreConfigures.Add(new Action<IBusRegistrationConfigurator>(c =>
                {
                    c.UsingInMemory();
                }));
            });

            var configuration = context.Services.GetConfiguration();
            PreConfigure<AbpMassTransitKafkaOptions>(options => options.PreConfigure(configuration));

            var kafkaOptions = context.Services.ExecutePreConfiguredActions<AbpMassTransitKafkaOptions>();

            PreConfigure<AbpMassTransitKafkaOptions>(options =>
            {
                options.DefaultTopicFormatFunc = KafkaUtil.TopicFormat;

                options.DefaultReceiveEndpointConfigure = new Action<IKafkaTopicReceiveEndpointConfigurator>(c =>
                {
                    c.ConcurrentMessageLimit = kafkaOptions.DefaultConcurrentMessageLimit;
                    c.MaxPollInterval = TimeSpan.FromMilliseconds(kafkaOptions.DefaultMaxPollInterval);
                    c.SessionTimeout = TimeSpan.FromSeconds(kafkaOptions.DefaultSessionTimeout);
                    c.EnableAutoOffsetStore = kafkaOptions.DefaultEnableAutoOffsetStore;
                    c.AutoOffsetReset = kafkaOptions.DefaultAutoOffsetReset;
                });

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
            if (massTransitOptions.Provider.Equals(MassTransitKafkaConsts.ProviderName, StringComparison.OrdinalIgnoreCase))
            {
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

                            //Kafka configures
                            foreach (var configure in kafkaOptions.KafkaConfigures)
                            {
                                configure(ctx, k);
                            }

                            foreach (var consumer in kafkaOptions.Consumers)
                            {
                                var topic = kafkaOptions.DefaultTopicFormatFunc(massTransitOptions.Prefix, consumer.Topic);

                                var groupId = consumer.GroupId.IsNullOrWhiteSpace() ?
                                kafkaOptions.DefaultGroupId : consumer.GroupId;

                                var receiveEndpointConfigure = consumer.ReceiveEndpointConfigure ?? kafkaOptions.DefaultReceiveEndpointConfigure;

                                consumer.TopicEndpointConfigure?.Invoke(topic, groupId, receiveEndpointConfigure, ctx, k);
                            }


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


        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpMassTransitKafkaOptions>(options =>
            {
                var actions = context.Services.GetPreConfigureActions<AbpMassTransitKafkaOptions>();
                foreach (var action in actions)
                {
                    action(options);
                }
            });
        }

    }
}
