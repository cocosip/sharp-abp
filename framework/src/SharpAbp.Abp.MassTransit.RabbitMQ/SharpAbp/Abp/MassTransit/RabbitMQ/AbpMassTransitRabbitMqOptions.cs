using MassTransit;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.MassTransit.RabbitMQ
{
    public class AbpMassTransitRabbitMqOptions
    {
        public string? Host { get; set; }
        public ushort Port { get; set; }
        public string? VirtualHost { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }

        /// <summary>
        /// ConnectionName
        /// </summary>
        public string? ConnectionName { get; set; }
        public bool UseSSL { get; set; }
        public bool UseCluster { get; set; }
        public List<string> ClusterNodes { get; set; }

        /// <summary>
        /// Queue prefix
        /// </summary>
        public string? DefaultQueuePrefix { get; set; }
        public int DefaultConcurrentMessageLimit { get; set; } = 1;
        public int DefaultPrefetchCount { get; set; } = 4;
        public bool DefaultDurable { get; set; } = true;
        public bool DefaultAutoDelete { get; set; } = false;
        public string DefaultExchangeType { get; set; } = ExchangeType.Fanout;

        public Action<IRabbitMqSslConfigurator>? ConfigureSsl { get; set; }
        public Func<string?, string, string>? DefaultExchangeNameFormatFunc { get; set; }
        public Func<string?, string?, string, string>? DefaultQueueNameFormatFunc { get; set; }

        public Action<IRabbitMqMessagePublishTopologyConfigurator>? DefaultPublishTopologyConfigure { get; set; }
        public Action<string, string, IRabbitMqReceiveEndpointConfigurator>? DefaultReceiveEndpointConfigure { get; set; }

        public List<Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>> RabbitMqPreConfigures { get; set; }
        public List<Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>> RabbitMqConfigures { get; set; }
        public List<Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>> RabbitMqPostConfigures { get; set; }
        public List<RabbitMqProducerConfiguration> Producers { get; set; }
        public List<RabbitMqConsumerConfiguration> Consumers { get; set; }

        public AbpMassTransitRabbitMqOptions()
        {
            ClusterNodes = [];

            RabbitMqPreConfigures = [];
            RabbitMqConfigures = [];
            RabbitMqPostConfigures = [];

            Producers = [];
            Consumers = [];
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
                UseSSL = massTransitRabbitMqOptions.UseSSL;
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

        public AbpMassTransitRabbitMqOptions CopyFrom(AbpMassTransitRabbitMqOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            Host = options.Host;
            Port = options.Port;
            VirtualHost = options.VirtualHost;
            Username = options.Username;
            Password = options.Password;
            ConnectionName = options.ConnectionName;
            UseSSL = options.UseSSL;
            UseCluster = options.UseCluster;
            ClusterNodes = [.. options.ClusterNodes];

            DefaultQueuePrefix = options.DefaultQueuePrefix;
            DefaultConcurrentMessageLimit = options.DefaultConcurrentMessageLimit;
            DefaultPrefetchCount = options.DefaultPrefetchCount;
            DefaultDurable = options.DefaultDurable;
            DefaultAutoDelete = options.DefaultAutoDelete;
            DefaultExchangeType = options.DefaultExchangeType;

            ConfigureSsl = options.ConfigureSsl;
            DefaultExchangeNameFormatFunc = options.DefaultExchangeNameFormatFunc;
            DefaultQueueNameFormatFunc = options.DefaultQueueNameFormatFunc;
            DefaultPublishTopologyConfigure = options.DefaultPublishTopologyConfigure;
            DefaultReceiveEndpointConfigure = options.DefaultReceiveEndpointConfigure;

            RabbitMqPreConfigures.Clear();
            RabbitMqPreConfigures.AddRange(options.RabbitMqPreConfigures);

            RabbitMqConfigures.Clear();
            RabbitMqConfigures.AddRange(options.RabbitMqConfigures);

            RabbitMqPostConfigures.Clear();
            RabbitMqPostConfigures.AddRange(options.RabbitMqPostConfigures);

            Producers.Clear();
            Producers.AddRange(options.Producers);

            Consumers.Clear();
            Consumers.AddRange(options.Consumers);

            return this;
        }


    }
}
