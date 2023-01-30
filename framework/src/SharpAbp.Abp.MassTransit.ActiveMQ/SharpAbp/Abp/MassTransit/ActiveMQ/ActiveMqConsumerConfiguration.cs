using MassTransit;
using System;

namespace SharpAbp.Abp.MassTransit.ActiveMQ
{
    public class ActiveMqConsumerConfiguration
    {

        /// <summary>
        /// QueueName
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// Message configure
        /// </summary>
        public Action<string, IActiveMqBusFactoryConfigurator> MessageConfigure { get; set; }

        /// <summary>
        /// Configure
        /// </summary>
        public Action<IBusRegistrationConfigurator> Configure { get; set; }

        /// <summary>
        /// ReceiveEndPoint configure
        /// </summary>
        public Action<string, IActiveMqReceiveEndpointConfigurator> ReceiveEndpointConfigure { get; set; }

        /// <summary>
        /// ReceiveEndPoint configure
        /// </summary>
        public Action<string, Action<string, IActiveMqReceiveEndpointConfigurator>, IBusRegistrationContext, IActiveMqBusFactoryConfigurator> ReceiveEndpoint { get; set; }

        public Action<Uri> MapConfigure { get; set; }
    }
}
