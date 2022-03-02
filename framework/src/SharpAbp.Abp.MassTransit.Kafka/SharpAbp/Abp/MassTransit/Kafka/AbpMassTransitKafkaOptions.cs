using MassTransit.KafkaIntegration;
using MassTransit.Registration;
using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.MassTransit.Kafka
{
    public class AbpMassTransitKafkaOptions
    {
        public string Server { get; set; }
        public bool UseSsl { get; set; }
        public KafkaTopicReceiveEndpointConfiguration DefaultReceiveEndpointConfiguration { get; set; }
        public Action<IKafkaSslConfigurator> ConfigureSsl { get; set; }
        public Func<string, string, string> DefaultTopicFormatFunc { get; set; }
        public Action<IKafkaTopicReceiveEndpointConfigurator> DefaultReceiveEndpointConfigure { get; set; }

        public List<Action<IRiderRegistrationConfigurator>> RiderPreConfigures { get; set; }
        public List<Action<IRiderRegistrationConfigurator>> RiderConfigures { get; set; }
        public List<Action<IRiderRegistrationConfigurator>> RiderPostConfigures { get; set; }

        public List<Action<IRiderRegistrationContext, IKafkaFactoryConfigurator>> KafkaPreConfigures { get; set; }
        public List<Action<IRiderRegistrationContext, IKafkaFactoryConfigurator>> KafkaConfigures { get; set; }
        public List<Action<IRiderRegistrationContext, IKafkaFactoryConfigurator>> KafkaPostConfigures { get; set; }

        public List<KafkaProducerConfiguration> Producers { get; set; }
        public List<KafkaConsumerConfiguration> Consumers { get; set; }

        public AbpMassTransitKafkaOptions()
        {
            DefaultReceiveEndpointConfiguration = new KafkaTopicReceiveEndpointConfiguration();

            RiderPreConfigures = new List<Action<IRiderRegistrationConfigurator>>();
            RiderConfigures = new List<Action<IRiderRegistrationConfigurator>>();
            RiderPostConfigures = new List<Action<IRiderRegistrationConfigurator>>();

            KafkaPreConfigures = new List<Action<IRiderRegistrationContext, IKafkaFactoryConfigurator>>();
            KafkaConfigures = new List<Action<IRiderRegistrationContext, IKafkaFactoryConfigurator>>();
            KafkaPostConfigures = new List<Action<IRiderRegistrationContext, IKafkaFactoryConfigurator>>();

            Producers = new List<KafkaProducerConfiguration>();
            Consumers = new List<KafkaConsumerConfiguration>();
        }


    }
}
