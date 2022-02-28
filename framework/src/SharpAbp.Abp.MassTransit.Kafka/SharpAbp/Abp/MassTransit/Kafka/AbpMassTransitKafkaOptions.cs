using MassTransit.KafkaIntegration;
using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.MassTransit.Kafka
{
    public class AbpMassTransitKafkaOptions
    {
        public string Server { get; set; }
        public bool UseSsl { get; set; }
        public Action<IKafkaSslConfigurator> ConfigureSsl { get; set; }
        public Action<IKafkaTopicReceiveEndpointConfigurator> DefaultReceiveEndpointConfigurator { get; set; }

        public List<KafkaProducerConfiguration> Producers { get; set; }
        public List<KafkaConsumerConfiguration> Consumers { get; set; }

        public AbpMassTransitKafkaOptions()
        {
            Producers = new List<KafkaProducerConfiguration>();
            Consumers = new List<KafkaConsumerConfiguration>();
        }
    }
}
