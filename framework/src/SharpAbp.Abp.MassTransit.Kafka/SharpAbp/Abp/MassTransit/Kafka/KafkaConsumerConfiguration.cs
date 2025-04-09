using MassTransit;
using System;

namespace SharpAbp.Abp.MassTransit.Kafka
{
    public class KafkaConsumerConfiguration
    {
        /// <summary>
        /// Consumer topic
        /// </summary>
        public string? Topic { get; set; }

        /// <summary>
        /// Group
        /// </summary>
        public string? GroupId { get; set; }

        /// <summary>
        /// Producer configure
        /// </summary>
        public Action<IRiderRegistrationConfigurator>? Configure { get; set; }

        /// <summary>
        /// Receive endpoint
        /// </summary>
        public Action<IKafkaTopicReceiveEndpointConfigurator>? ReceiveEndpointConfigure { get; set; }

        /// <summary>
        /// TopicEndPoint configure
        /// </summary>
        public Action<string, string, Action<IKafkaTopicReceiveEndpointConfigurator>, IRiderRegistrationContext, IKafkaFactoryConfigurator>? TopicEndpointConfigure { get; set; }

    }
}
