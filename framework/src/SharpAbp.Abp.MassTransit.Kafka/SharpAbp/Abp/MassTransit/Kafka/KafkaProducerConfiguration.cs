using MassTransit.Registration;
using System;

namespace SharpAbp.Abp.MassTransit.Kafka
{
    public class KafkaProducerConfiguration
    {
        /// <summary>
        /// Topic name
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// Producer configuration
        /// </summary>
        public Action<string, IRiderRegistrationConfigurator> Configure { get; set; }
    }
}
