using MassTransit;
using MassTransitSample.Common;
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
                    TopicEndpointConfigure = new Action<string, string, Action<IKafkaTopicReceiveEndpointConfigurator>, IRiderRegistrationContext, IKafkaFactoryConfigurator>((topic, groupId, preConfigure, ctx, cfg) =>
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
