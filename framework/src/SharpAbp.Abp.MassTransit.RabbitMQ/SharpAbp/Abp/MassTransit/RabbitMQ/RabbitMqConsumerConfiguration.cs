using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.RabbitMqTransport;
using System;

namespace SharpAbp.Abp.MassTransit.RabbitMQ
{
    public class RabbitMqConsumerConfiguration
    {
        /// <summary>
        /// EntityName
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// Configure
        /// </summary>
        public Action<IServiceCollectionBusConfigurator> Configure { get; set; }

        /// <summary>
        /// ReceiveEndPoint configure
        /// </summary>
        public Action<IReceiveEndpointConfigurator> ReceiveEndpointConfigure { get; set; }

        /// <summary>
        /// ReceiveEndPoint configure
        /// </summary>
        public Action<string, Action<IReceiveEndpointConfigurator>, IBusRegistrationContext, IRabbitMqBusFactoryConfigurator> ReceiveEndpoint { get; set; }
    }
}
