using MassTransit;
using MassTransit.RabbitMqTransport;
using MassTransit.RabbitMqTransport.Topology;
using System;

namespace SharpAbp.Abp.MassTransit.RabbitMQ
{
    public class RabbitMqProducerConfiguration
    {
        /// <summary>
        /// EntityName
        /// </summary>
        public string ExchangeName { get; set; }

        /// <summary>
        /// Message configure
        /// </summary>
        public Action<string, IRabbitMqBusFactoryConfigurator> MessageConfigure { get; set; }

        /// <summary>
        /// Publish configure
        /// </summary>
        public Action<IRabbitMqMessagePublishTopologyConfigurator> PublishTopologyConfigure { get; set; }

        /// <summary>
        /// Publish configure
        /// </summary>
        public Action<Action<IRabbitMqMessagePublishTopologyConfigurator>, IBusRegistrationContext, IRabbitMqBusFactoryConfigurator> PublishConfigure { get; set; }
    }
}
