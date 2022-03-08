using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.RabbitMqTransport;
using MassTransitSample.Common;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.MassTransit.RabbitMQ;
using System;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace MassTransitSample.RabbitMqConsumer
{
    [DependsOn(
        typeof(MassTransitSampleCommonModule),
        typeof(AbpMassTransitRabbitMqModule),
        typeof(AbpAutofacModule)
        )]
    public class MassTransitSampleRabbitMqConsumerModule : AbpModule
    {

        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            PreConfigure<AbpMassTransitRabbitMqOptions>(options =>
            {
                options.Consumers.Add(new RabbitMqConsumerConfiguration()
                {
                    ExchangeName = RabbitMqQueues.Exchange1,
                    QueueName = RabbitMqQueues.Queue1,
                    Configure = new Action<IServiceCollectionBusConfigurator>(c =>
                    {
                        c.AddConsumer<RabbitMqMessageConsumer>();
                    }),
                    MessageConfigure = new Action<string, IRabbitMqBusFactoryConfigurator>((exchangeName, c) =>
                    {
                        c.Message<RabbitMqMessage>(e =>
                        {
                            e.SetEntityName(exchangeName);
                        });
                    }),
                    ReceiveEndpoint = new Action<string, string, Action<string, string, IRabbitMqReceiveEndpointConfigurator>, IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>((exchangeName, queueName, preConfigure, ctx, cfg) =>
                    {
                        cfg.ReceiveEndpoint(queueName, e =>
                        {
                            preConfigure?.Invoke(exchangeName, queueName, e);
                            e.Consumer<RabbitMqMessageConsumer>(ctx);
                        });

                    })

                });

            });
        }


        public override void PostConfigureServices(ServiceConfigurationContext context)
        {

            var s = context.Services.ExecutePreConfiguredActions<AbpMassTransitRabbitMqOptions>();

            var cfgs = s.GetMessageConfigures();


            context.Services.AddHostedService<MassTransitSampleRabbitMqConsumerHostedService>();
        }
    }
}
