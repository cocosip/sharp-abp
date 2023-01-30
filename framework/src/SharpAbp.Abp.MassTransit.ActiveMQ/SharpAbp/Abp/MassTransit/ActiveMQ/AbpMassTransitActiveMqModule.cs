using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.MassTransit.ActiveMQ
{
    [DependsOn(
        typeof(AbpMassTransitModule)
        )]
    public class AbpMassTransitActiveMqModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            PreConfigure<AbpMassTransitActiveMqOptions>(options => options.PreConfigure(configuration));

            var activeMqOptions = context.Services.ExecutePreConfiguredActions<AbpMassTransitActiveMqOptions>();

            PreConfigure<AbpMassTransitActiveMqOptions>(options =>
            {
                options.DefaultQueueNameFormatFunc = ActiveMqUtil.QueueNameFormat;

                options.DefaultPublishTopologyConfigure = new Action<IActiveMqMessagePublishTopologyConfigurator>(c =>
                {
                    c.AutoDelete = activeMqOptions.DefaultAutoDelete;
                    c.Durable = activeMqOptions.DefaultDurable;
                    c.Exclude = activeMqOptions.DefaultExclude;
                });

                options.DefaultReceiveEndpointConfigure = new Action<string, IActiveMqReceiveEndpointConfigurator>((queueName, c) =>
                {
                    //c.Bind(exchangeName);
                    c.ConcurrentMessageLimit = activeMqOptions.DefaultConcurrentMessageLimit;
                    c.PrefetchCount = activeMqOptions.DefaultPrefetchCount;
                    c.AutoDelete = activeMqOptions.DefaultAutoDelete;
                    c.Durable = activeMqOptions.DefaultDurable;
                    //c.ExchangeType = rabbitMqOptions.DefaultExchangeType;
                });


                options.ActiveMqPostConfigures.Add(new Action<IBusRegistrationContext, IActiveMqBusFactoryConfigurator>((ctx, cfg) =>
                {
                    cfg.ConfigureEndpoints(ctx);
                }));
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var massTransitOptions = context.Services.ExecutePreConfiguredActions<AbpMassTransitOptions>();

            if (massTransitOptions.Provider.Equals(MassTransitActiveMqConsts.ProviderName, StringComparison.OrdinalIgnoreCase))
            {
                var activeMqOptions = context.Services.ExecutePreConfiguredActions<AbpMassTransitActiveMqOptions>();

                context.Services.AddMassTransit(x =>
                {
                    //Masstransit preConfigure
                    foreach (var preConfigure in massTransitOptions.PreConfigures)
                    {
                        preConfigure(x);
                    }

                    //Consumer
                    foreach (var consumer in activeMqOptions.Consumers)
                    {
                        consumer.Configure?.Invoke(x);
                    }

                    //Get message configures
                    var messageConfigues = activeMqOptions.GetMessageConfigures();

                    x.UsingActiveMq((ctx, cfg) =>
                    {
                        //ActiveMq preConfigure
                        foreach (var preConfigure in activeMqOptions.ActiveMqPreConfigures)
                        {
                            preConfigure(ctx, cfg);
                        }

                        cfg.Host(activeMqOptions.Host, activeMqOptions.Port, h =>
                        {
                            h.Username(activeMqOptions.Username);
                            h.Password(activeMqOptions.Password);

                            if (activeMqOptions.UseSsl)
                            {
                                h.UseSsl();
                            }

                        });

                        //ActiveMq configure
                        foreach (var configure in activeMqOptions.ActiveMqConfigures)
                        {
                            configure(ctx, cfg);
                        }

                        //Message configure
                        foreach (var messageConfigue in messageConfigues)
                        {
                            var queueName = activeMqOptions.DefaultQueueNameFormatFunc(massTransitOptions.Prefix, messageConfigue.Item1);
                            messageConfigue.Item2?.Invoke(queueName, cfg);
                        }

                        //Producer
                        foreach (var producer in activeMqOptions.Producers)
                        {
                            var queueName = activeMqOptions.DefaultQueueNameFormatFunc(massTransitOptions.Prefix, producer.QueueName);
                            producer.MapConfigure?.Invoke(new Uri($"activemq://{activeMqOptions.Host}/{queueName}"));

                            var publishTopologyConfigure = producer.PublishTopologyConfigure ?? activeMqOptions.DefaultPublishTopologyConfigure;
                            producer.PublishConfigure?.Invoke(publishTopologyConfigure, ctx, cfg);
                        }

                        //Consumer
                        foreach (var consumer in activeMqOptions.Consumers)
                        {
                            var queueName = activeMqOptions.DefaultQueueNameFormatFunc(massTransitOptions.Prefix, consumer.QueueName);
                            consumer.MapConfigure?.Invoke(new Uri($"activemq://{activeMqOptions.Host}/{queueName}"));

                            var receiveEndpointConfigure = consumer.ReceiveEndpointConfigure ?? activeMqOptions.DefaultReceiveEndpointConfigure;
                            consumer.ReceiveEndpoint?.Invoke(queueName, receiveEndpointConfigure, ctx, cfg);
                        }

                        if (activeMqOptions.DefaultEnableArtemisCompatibility)
                        {
                            cfg.EnableArtemisCompatibility();
                        }

                        //RabbitMq postConfigure
                        foreach (var postConfigure in activeMqOptions.ActiveMqPostConfigures)
                        {
                            postConfigure(ctx, cfg);
                        }

                    });


                });
            }
        }

        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpMassTransitActiveMqOptions>(options =>
            {
                var actions = context.Services.GetPreConfigureActions<AbpMassTransitActiveMqOptions>();
                foreach (var action in actions)
                {
                    action(options);
                }
            });
        }
    }
}
