using MassTransit;
using MassTransitSample.Common;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.MassTransit.ActiveMQ;
using System;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace MassTransitSample.ActiveMqProducer
{
    [DependsOn(
        typeof(MassTransitSampleCommonModule),
        typeof(AbpMassTransitActiveMqModule),
        typeof(AbpAutofacModule)
        )]
    public class MassTransitSampleActiveMqProducerModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            PreConfigure<AbpMassTransitActiveMqOptions>(options =>
            {
                options.Producers.Add(new ActiveMqProducerConfiguration()
                {
                    QueueName = ActiveMqQueues.Queue1,

                    PublishConfigure = new Action<Action<IActiveMqMessagePublishTopologyConfigurator>, IBusRegistrationContext, IActiveMqBusFactoryConfigurator>((p, ctx, cfg) =>
                    {
                        cfg.Publish<ActiveMqMessage>(c =>
                        {
                            p?.Invoke(c);
                        });
                    }),

                    MapConfigure = new Action<Uri>(u =>
                    {
                        EndpointConvention.Map<ActiveMqMessage>(u);
                    }),

                });

            });
        }


        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddHostedService<MassTransitSampleActiveMqProducerHostedService>();
        }
    }
}
