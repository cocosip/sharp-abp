using MassTransit;
using MassTransit.RabbitMqTransport;
using MassTransit.RabbitMqTransport.Topology;
using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.MassTransit.RabbitMQ
{
    public class AbpMassTransitRabbitMqOptions
    {
        public string Host { get; set; }
        public ushort Port { get; set; }
        public string VirtualHost { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool UseSsl { get; set; }
        public Action<IRabbitMqSslConfigurator> ConfigureSsl { get; set; }
        public Func<string, string, string> DefaultEntityNameFormatFunc { get; set; }
        public Action<IRabbitMqMessagePublishTopologyConfigurator> DefaultPublishTopologyConfigure { get; set; }

        public Action<IReceiveEndpointConfigurator> DefaultReceiveEndpointConfigure { get; set; }

        public List<Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>> RabbitMqPreConfigures { get; set; }
        public List<Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>> RabbitMqConfigures { get; set; }
        public List<Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>> RabbitMqPostConfigures { get; set; }

        public List<RabbitMqProducerConfiguration> Producers { get; set; }
        public List<RabbitMqConsumerConfiguration> Consumers { get; set; }

        public AbpMassTransitRabbitMqOptions()
        {
            RabbitMqPreConfigures = new List<Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>>();
            RabbitMqConfigures = new List<Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>>();
            RabbitMqPostConfigures = new List<Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>>();

            Producers = new List<RabbitMqProducerConfiguration>();
            Consumers = new List<RabbitMqConsumerConfiguration>();
        }
    }
}
