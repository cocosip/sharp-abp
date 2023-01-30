using MassTransit;
using MassTransitSample.Common;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.MassTransit.ActiveMQ;
using System;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace MassTransitSample.ActiveMqConsumer
{
    [DependsOn(
        typeof(MassTransitSampleCommonModule),
        typeof(AbpMassTransitActiveMqModule),
        typeof(AbpAutofacModule)
        )]
    public class MassTransitSampleActiveMqConsumerModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            PreConfigure<AbpMassTransitActiveMqOptions>(options =>
            {
                options.Consumers.Add(new ActiveMqConsumerConfiguration()
                {
                    QueueName = ActiveMqQueues.Queue1,
                    Configure = new Action<IBusRegistrationConfigurator>(c =>
                    {
                        c.AddConsumer<ActiveMqMessageConsumer>();
                    }),


                    ReceiveEndpoint = new Action<string, Action<string, IActiveMqReceiveEndpointConfigurator>, IBusRegistrationContext, IActiveMqBusFactoryConfigurator>((queueName, preConfigure, ctx, cfg) =>
                    {
                        cfg.ReceiveEndpoint(queueName, e =>
                        {
                            preConfigure?.Invoke(queueName, e);
                            e.ConfigureConsumer<ActiveMqMessageConsumer>(ctx);
                        });
                    }),
                    MapConfigure = new Action<Uri>(u =>
                    {
                        EndpointConvention.Map<ActiveMqMessage>(u);
                    })
                });

            });
        }


        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddHostedService<MassTransitSampleActiveMqConsumerHostedService>();
        }
    }
}
