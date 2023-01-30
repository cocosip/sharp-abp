using MassTransit;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.MassTransit.ActiveMQ
{
    public class AbpMassTransitActiveMqOptions
    {
        public string Host { get; set; }
        public ushort Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool UseSsl { get; set; }

        public int DefaultConcurrentMessageLimit { get; set; } = 1;
        public int DefaultPrefetchCount { get; set; } = 4;
        public bool DefaultDurable { get; set; } = true;
        public bool DefaultAutoDelete { get; set; } = false;
        public bool DefaultExclude { get; set; } = false;
        public bool DefaultEnableArtemisCompatibility { get; set; } = true;


        public Func<string, string, string> DefaultQueueNameFormatFunc { get; set; }

        public Action<IActiveMqMessagePublishTopologyConfigurator> DefaultPublishTopologyConfigure { get; set; }
        public Action<string, IActiveMqReceiveEndpointConfigurator> DefaultReceiveEndpointConfigure { get; set; }

        public List<Action<IBusRegistrationContext, IActiveMqBusFactoryConfigurator>> ActiveMqPreConfigures { get; set; }
        public List<Action<IBusRegistrationContext, IActiveMqBusFactoryConfigurator>> ActiveMqConfigures { get; set; }
        public List<Action<IBusRegistrationContext, IActiveMqBusFactoryConfigurator>> ActiveMqPostConfigures { get; set; }

        public List<ActiveMqProducerConfiguration> Producers { get; set; }
        public List<ActiveMqConsumerConfiguration> Consumers { get; set; }

        public AbpMassTransitActiveMqOptions()
        {
            ActiveMqPreConfigures = new List<Action<IBusRegistrationContext, IActiveMqBusFactoryConfigurator>>();
            ActiveMqConfigures = new List<Action<IBusRegistrationContext, IActiveMqBusFactoryConfigurator>>();
            ActiveMqPostConfigures = new List<Action<IBusRegistrationContext, IActiveMqBusFactoryConfigurator>>();

            Producers = new List<ActiveMqProducerConfiguration>();
            Consumers = new List<ActiveMqConsumerConfiguration>();
        }

        public AbpMassTransitActiveMqOptions PreConfigure(IConfiguration configuration)
        {
            var massTransitRabbitMqOptions = configuration
                .GetSection("MassTransitOptions:ActiveMqOptions")
                .Get<AbpMassTransitActiveMqOptions>();

            if (massTransitRabbitMqOptions != null)
            {
                Host = massTransitRabbitMqOptions.Host;
                Port = massTransitRabbitMqOptions.Port;
                Username = massTransitRabbitMqOptions.Username;
                Password = massTransitRabbitMqOptions.Password;
                UseSsl = massTransitRabbitMqOptions.UseSsl;

                DefaultConcurrentMessageLimit = massTransitRabbitMqOptions.DefaultConcurrentMessageLimit;
                DefaultPrefetchCount = massTransitRabbitMqOptions.DefaultPrefetchCount;
                DefaultDurable = massTransitRabbitMqOptions.DefaultDurable;
                DefaultAutoDelete = massTransitRabbitMqOptions.DefaultAutoDelete;
                DefaultExclude = massTransitRabbitMqOptions.DefaultExclude;
                DefaultEnableArtemisCompatibility = massTransitRabbitMqOptions.DefaultEnableArtemisCompatibility;
            }

            return this;
        }
    }
}
