using MassTransitSample.Common;
using SharpAbp.Abp.MassTransit.RabbitMQ;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;
using System;
using MassTransit.RabbitMqTransport;
using MassTransit.RabbitMqTransport.Topology;
using MassTransit;

namespace MassTransitSample.RabbitMqProducer
{
    [DependsOn(
        typeof(MassTransitSampleCommonModule),
        typeof(AbpMassTransitRabbitMqModule),
        typeof(AbpAutofacModule)
        )]
    public class MassTransitSampleRabbitMqProducerModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            PreConfigure<AbpMassTransitRabbitMqOptions>(options =>
            {
                options.Producers.Add(new RabbitMqProducerConfiguration()
                {
                    ExchangeName = RabbitMqQueues.Exchange1,
                    MessageConfigure = new Action<string, IRabbitMqBusFactoryConfigurator>((exchangeName, c) =>
                    {
                        c.Message<RabbitMqMessage>(e =>
                        {
                            e.SetEntityName(exchangeName);
                        });

                    }),
                    PublishConfigure = new Action<Action<IRabbitMqMessagePublishTopologyConfigurator>, IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>((preConfigure, ctx, cfg) =>
                    {
                        cfg.Publish<RabbitMqMessage>(c =>
                        {
                            preConfigure?.Invoke(c);
                        });
                    })

                });

            });
        }


        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddHostedService<MassTransitSampleRabbitMqProducerHostedService>();
        }

    }
}
