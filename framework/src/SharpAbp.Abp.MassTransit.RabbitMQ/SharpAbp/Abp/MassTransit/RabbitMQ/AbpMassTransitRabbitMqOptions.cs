using MassTransit;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
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

        /// <summary>
        /// ConnectionName
        /// </summary>
        public string ConnectionName { get; set; }
        public bool UseSsl { get; set; }
        public bool UseCluster { get; set; }
        public List<string> ClusterNodes { get; set; }

        /// <summary>
        /// Queue prefix
        /// </summary>
        public string DefaultQueuePrefix { get; set; }
        public int DefaultConcurrentMessageLimit { get; set; } = 1;
        public int DefaultPrefetchCount { get; set; } = 4;
        public bool DefaultDurable { get; set; } = true;
        public bool DefaultAutoDelete { get; set; } = false;
        public string DefaultExchangeType { get; set; } = ExchangeType.Fanout;

        public Action<IRabbitMqSslConfigurator> ConfigureSsl { get; set; }
        public Func<string, string, string> DefaultExchangeNameFormatFunc { get; set; }
        public Func<string, string, string, string> DefaultQueueNameFormatFunc { get; set; }

        public Action<IRabbitMqMessagePublishTopologyConfigurator> DefaultPublishTopologyConfigure { get; set; }
        public Action<string, string, IRabbitMqReceiveEndpointConfigurator> DefaultReceiveEndpointConfigure { get; set; }

        public List<Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>> RabbitMqPreConfigures { get; set; }
        public List<Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>> RabbitMqConfigures { get; set; }
        public List<Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>> RabbitMqPostConfigures { get; set; }
        public List<RabbitMqProducerConfiguration> Producers { get; set; }
        public List<RabbitMqConsumerConfiguration> Consumers { get; set; }

        public AbpMassTransitRabbitMqOptions()
        {
            ClusterNodes = new List<string>();

            RabbitMqPreConfigures = new List<Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>>();
            RabbitMqConfigures = new List<Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>>();
            RabbitMqPostConfigures = new List<Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>>();

            Producers = new List<RabbitMqProducerConfiguration>();
            Consumers = new List<RabbitMqConsumerConfiguration>();
        }

        public AbpMassTransitRabbitMqOptions PreConfigure(IConfiguration configuration)
        {
            var massTransitRabbitMqOptions = configuration
                .GetSection("MassTransitOptions:RabbitMqOptions")
                .Get<AbpMassTransitRabbitMqOptions>();

            if (massTransitRabbitMqOptions != null)
            {
                Host = massTransitRabbitMqOptions.Host;
                Port = massTransitRabbitMqOptions.Port;
                VirtualHost = massTransitRabbitMqOptions.VirtualHost;
                Username = massTransitRabbitMqOptions.Username;
                Password = massTransitRabbitMqOptions.Password;
                ConnectionName = massTransitRabbitMqOptions.ConnectionName;
                UseSsl = massTransitRabbitMqOptions.UseSsl;
                UseCluster = massTransitRabbitMqOptions.UseCluster;
                ClusterNodes = massTransitRabbitMqOptions.ClusterNodes;

                DefaultQueuePrefix = massTransitRabbitMqOptions.DefaultQueuePrefix;
                DefaultConcurrentMessageLimit = massTransitRabbitMqOptions.DefaultConcurrentMessageLimit;
                DefaultPrefetchCount = massTransitRabbitMqOptions.DefaultPrefetchCount;
                DefaultDurable = massTransitRabbitMqOptions.DefaultDurable;
                DefaultAutoDelete = massTransitRabbitMqOptions.DefaultAutoDelete;
                DefaultExchangeType = massTransitRabbitMqOptions.DefaultExchangeType;

            }

            return this;
        }


    }
}
