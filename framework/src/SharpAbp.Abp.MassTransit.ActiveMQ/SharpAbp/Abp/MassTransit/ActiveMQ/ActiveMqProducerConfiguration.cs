using MassTransit;
using System;

namespace SharpAbp.Abp.MassTransit.ActiveMQ
{
    public class ActiveMqProducerConfiguration
    {
        /// <summary>
        /// QueueName
        /// </summary>
        public string? QueueName { get; set; }

        /// <summary>
        /// Message configure
        /// </summary>
        public Action<string, IActiveMqBusFactoryConfigurator>? MessageConfigure { get; set; }

        /// <summary>
        /// Publish configure
        /// </summary>
        public Action<IActiveMqMessagePublishTopologyConfigurator>? PublishTopologyConfigure { get; set; }

        /// <summary>
        /// Publish configure
        /// </summary>
        public Action<Action<IActiveMqMessagePublishTopologyConfigurator>, IBusRegistrationContext, IActiveMqBusFactoryConfigurator>? PublishConfigure { get; set; }

        public Action<Uri>? MapConfigure { get; set; }
    }
}
