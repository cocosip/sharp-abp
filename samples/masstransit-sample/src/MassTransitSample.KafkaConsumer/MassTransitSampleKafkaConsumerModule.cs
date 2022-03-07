using MassTransit;
using MassTransit.KafkaIntegration;
using MassTransit.Registration;
using MassTransitSample.Common;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.MassTransit;
using SharpAbp.Abp.MassTransit.Kafka;
using System;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace MassTransitSample.KafkaConsumer
{
    [DependsOn(
        typeof(MassTransitSampleCommonModule),
        typeof(AbpMassTransitKafkaModule),
        typeof(AbpAutofacModule)
     )]
    public class MassTransitSampleKafkaConsumerModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {


            PreConfigure<AbpMassTransitKafkaOptions>(options =>
            {
                options.Consumers.Add(new KafkaConsumerConfiguration()
                {
                    Topic = KafkaTopics.Topic1,
                    Configure = new Action<IRiderRegistrationConfigurator>(c =>
                    {
                        c.AddConsumer<KafkaMessageConsumer>();
                    }),
                    TopicEndpointConfigure = new Action<string, string, Action<IKafkaTopicReceiveEndpointConfigurator>, MassTransit.Registration.IRiderRegistrationContext, IKafkaFactoryConfigurator>((topic, groupId, preConfigure, ctx, cfg) =>
                     {
                         cfg.TopicEndpoint<string, KafkaMessage>(topic, groupId, e =>
                         {
                             preConfigure?.Invoke(e);
                             e.ConfigureConsumer<KafkaMessageConsumer>(ctx);
                         });

                     })
                });
            });
        }

    }
}
