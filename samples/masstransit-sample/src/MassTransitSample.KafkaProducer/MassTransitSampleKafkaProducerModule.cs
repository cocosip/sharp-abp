using MassTransit.KafkaIntegration;
using MassTransit.Registration;
using MassTransitSample.Common;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.MassTransit;
using SharpAbp.Abp.MassTransit.Kafka;
using System;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace MassTransitSample.KafkaProducer
{
    [DependsOn(
        typeof(MassTransitSampleCommonModule),
        typeof(AbpMassTransitKafkaModule),
        typeof(AbpAutofacModule)
        )]
    public class MassTransitSampleKafkaProducerModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            PreConfigure<AbpMassTransitKafkaOptions>(options =>
            {
                options.Producers.Add(new KafkaProducerConfiguration()
                {
                    Topic = KafkaTopics.Topic1,
                    Configure = new Action<string, IRiderRegistrationConfigurator>((topic, c) =>
                    {
                        c.AddProducer<string, KafkaMessage>(topic);
                    })
                });
            });
        }


        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddHostedService<MassTransitSampleKafkaProducerHostedService>();
        }

    }
}
