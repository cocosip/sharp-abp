using MassTransit.KafkaIntegration;
using MassTransit.Registration;
using System;

namespace SharpAbp.Abp.MassTransit.Kafka
{
    public class KafkaConsumerConfiguration
    {
        /// <summary>
        /// Consumer topic
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// Producer configure
        /// </summary>
        public Action<IRiderRegistrationConfigurator> Configurator { get; set; }

        /// <summary>
        /// Receive endpoint
        /// </summary>
        public Action<IKafkaTopicReceiveEndpointConfigurator> ReceiveEndpointConfigurator { get; set; }

        /// <summary>
        /// TopicEndPoint configure
        /// </summary>
        public Action<string, string, Action<IKafkaTopicReceiveEndpointConfigurator>, IRiderRegistrationContext, IKafkaFactoryConfigurator> TopicEndpointConfigurator { get; set; }

    }
}
