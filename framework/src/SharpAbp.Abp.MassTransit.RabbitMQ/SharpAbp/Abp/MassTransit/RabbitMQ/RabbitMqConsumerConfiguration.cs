using MassTransit;
using System;

namespace SharpAbp.Abp.MassTransit.RabbitMQ
{
    public class RabbitMqConsumerConfiguration
    {
        /// <summary>
        /// EntityName
        /// </summary>
        public string ExchangeName { get; set; }

        /// <summary>
        /// QueueName
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// Message configure
        /// </summary>
        public Action<string, IRabbitMqBusFactoryConfigurator> MessageConfigure { get; set; }

        /// <summary>
        /// Configure
        /// </summary>
        public Action<IBusRegistrationConfigurator> Configure { get; set; }

        /// <summary>
        /// ReceiveEndPoint configure
        /// </summary>
        public Action<string, string, IRabbitMqReceiveEndpointConfigurator> ReceiveEndpointConfigure { get; set; }

        /// <summary>
        /// ReceiveEndPoint configure
        /// </summary>
        public Action<string, string, Action<string, string, IRabbitMqReceiveEndpointConfigurator>, IBusRegistrationContext, IRabbitMqBusFactoryConfigurator> ReceiveEndpoint { get; set; }
    }
}
