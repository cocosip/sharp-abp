using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.RabbitMqTransport;
using MassTransit.RabbitMqTransport.Topology;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.MassTransit.RabbitMQ
{
    [DependsOn(
        typeof(AbpMassTransitModule)
        )]
    public class AbpMassTransitRabbitMqModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            PreConfigure<AbpMassTransitRabbitMqOptions>(options =>
            {
                options.DefaultEntityNameFormatFunc = RabbitMqUtil.EntityNameFormat;

                options.RabbitMqPostConfigures.Add(new Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>((ctx, cfg) =>
                {
                    cfg.ConfigureEndpoints(ctx);
                }));
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {

            var massTransitOptions = context.Services.ExecutePreConfiguredActions<AbpMassTransitOptions>();

            var rabbitMqOptions = context.Services.ExecutePreConfiguredActions<AbpMassTransitRabbitMqOptions>();

            context.Services.AddMassTransit(x =>
            {
                //Masstransit preConfigure
                foreach (var preConfigure in massTransitOptions.PreConfigures)
                {
                    preConfigure(x);
                }

                //Consumer
                foreach (var consumer in rabbitMqOptions.Consumers)
                {
                    consumer.Configure?.Invoke(x);
                }


                x.UsingRabbitMq((ctx, cfg) =>
                {
                    //RabbitMq preConfigure
                    foreach (var preConfigure in rabbitMqOptions.RabbitMqPreConfigures)
                    {
                        preConfigure(ctx, cfg);
                    }

                    cfg.Host(rabbitMqOptions.Host, rabbitMqOptions.Port, rabbitMqOptions.VirtualHost, h =>
                    {
                        h.Username(rabbitMqOptions.Username);
                        h.Password(rabbitMqOptions.Password);
                        //SSL
                        if (rabbitMqOptions.UseSsl && rabbitMqOptions.ConfigureSsl != null)
                        {
                            h.UseSsl(rabbitMqOptions.ConfigureSsl);
                        }
                    });

                    //RabbitMq configure
                    foreach (var configure in rabbitMqOptions.RabbitMqConfigures)
                    {
                        configure(ctx, cfg);
                    }

                    //Producer
                    foreach (var producer in rabbitMqOptions.Producers)
                    {
                        var entityName = rabbitMqOptions.DefaultEntityNameFormatFunc(massTransitOptions.Prefix, producer.EntityName);
                        producer.MessageConfigure?.Invoke(entityName, cfg);

                        var publishTopologyConfigure = producer.PublishTopologyConfigure ?? rabbitMqOptions.DefaultPublishTopologyConfigure;

                        producer.PublishConfigure?.Invoke(publishTopologyConfigure, ctx, cfg);
                    }

                    //Consumer
                    foreach (var consumer in rabbitMqOptions.Consumers)
                    {
                        var entityName = rabbitMqOptions.DefaultEntityNameFormatFunc(massTransitOptions.Prefix, consumer.EntityName);

                        //var receiveEndpointConfigure = consumer.ReceiveEndpointConfigure ?? rabbitMqOptions.DefaultReceiveEndpointConfigure;

                        //consumer.ReceiveEndpointConfigure?.Invoke(entityName,, ctx, cfg);
                    }


                    cfg.ReceiveEndpoint("", c =>
                    {
                        c.Durable = true;
                        c.AutoDelete = true;

                        c.Bind("");
                    });

                    //RabbitMq postConfigure
                    foreach (var postConfigure in rabbitMqOptions.RabbitMqPostConfigures)
                    {
                        postConfigure(ctx, cfg);
                    }
                });

            });

            var o = new AbpMassTransitRabbitMqOptions();
            o.Producers.Add(new RabbitMqProducerConfiguration()
            {
                EntityName = "Entity1",
                MessageConfigure = new Action<string, IRabbitMqBusFactoryConfigurator>((entityName, cfg) =>
                {
                    cfg.Message<Class111>(c =>
                    {
                        c.SetEntityName(entityName);
                    });
                }),
                PublishConfigure = new Action<Action<IRabbitMqMessagePublishTopologyConfigurator>, IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>((action, ctx, cfg) =>
                {
                    cfg.Publish<Class111>(c =>
                    {
                        action(c);
                    });
                })
            });


            o.Consumers.Add(new RabbitMqConsumerConfiguration()
            {
                EntityName = "Entity1",
                Configure = new Action<IServiceCollectionBusConfigurator>(c =>
                {
                    c.AddConsumer<Class111Consumer>();
                }),
                ReceiveEndpoint = new Action<string, Action<IReceiveEndpointConfigurator>, IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>((entityName, action, ctx, cfg) =>
                  {
                      cfg.ReceiveEndpoint(entityName, e =>
                      {
                          action?.Invoke(e);
                          e.Durable = true;
                          e.ExchangeType = ExchangeType.Fanout;
                          e.Consumer<Class111Consumer>(ctx);
                      });
                  })
            });


        }

    }
}

public class Class111
{

}

public class Class111Consumer : IConsumer
{

}
