using System;
using System.Collections.Generic;
using MassTransit;
using Microsoft.Extensions.Configuration;

namespace SharpAbp.Abp.MassTransit.ActiveMQ
{
    public class AbpMassTransitActiveMqOptions
    {
        public string? Host { get; set; }
        public ushort Port { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public bool UseSSL { get; set; }

        public int DefaultConcurrentMessageLimit { get; set; } = 1;
        public int DefaultPrefetchCount { get; set; } = 4;
        public bool DefaultDurable { get; set; } = true;
        public bool DefaultAutoDelete { get; set; } = false;
        public bool DefaultExclude { get; set; } = false;
        public bool DefaultEnableArtemisCompatibility { get; set; } = true;

        public Func<string?, string, string>? DefaultQueueNameFormatFunc { get; set; }

        public Action<IActiveMqMessagePublishTopologyConfigurator>? DefaultPublishTopologyConfigure { get; set; }
        public Action<string, IActiveMqReceiveEndpointConfigurator>? DefaultReceiveEndpointConfigure { get; set; }

        public List<Action<IBusRegistrationContext, IActiveMqBusFactoryConfigurator>> ActiveMqPreConfigures { get; set; }
        public List<Action<IBusRegistrationContext, IActiveMqBusFactoryConfigurator>> ActiveMqConfigures { get; set; }
        public List<Action<IBusRegistrationContext, IActiveMqBusFactoryConfigurator>> ActiveMqPostConfigures { get; set; }

        public List<ActiveMqProducerConfiguration> Producers { get; set; }
        public List<ActiveMqConsumerConfiguration> Consumers { get; set; }

        public AbpMassTransitActiveMqOptions()
        {
            ActiveMqPreConfigures = [];
            ActiveMqConfigures = [];
            ActiveMqPostConfigures = [];

            Producers = [];
            Consumers = [];
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
                UseSSL = massTransitRabbitMqOptions.UseSSL;

                DefaultConcurrentMessageLimit = massTransitRabbitMqOptions.DefaultConcurrentMessageLimit;
                DefaultPrefetchCount = massTransitRabbitMqOptions.DefaultPrefetchCount;
                DefaultDurable = massTransitRabbitMqOptions.DefaultDurable;
                DefaultAutoDelete = massTransitRabbitMqOptions.DefaultAutoDelete;
                DefaultExclude = massTransitRabbitMqOptions.DefaultExclude;
                DefaultEnableArtemisCompatibility = massTransitRabbitMqOptions.DefaultEnableArtemisCompatibility;
            }

            return this;
        }

        public AbpMassTransitActiveMqOptions CopyFrom(AbpMassTransitActiveMqOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            Host = options.Host;
            Port = options.Port;
            Username = options.Username;
            Password = options.Password;
            UseSSL = options.UseSSL;

            DefaultConcurrentMessageLimit = options.DefaultConcurrentMessageLimit;
            DefaultPrefetchCount = options.DefaultPrefetchCount;
            DefaultDurable = options.DefaultDurable;
            DefaultAutoDelete = options.DefaultAutoDelete;
            DefaultExclude = options.DefaultExclude;
            DefaultEnableArtemisCompatibility = options.DefaultEnableArtemisCompatibility;

            DefaultQueueNameFormatFunc = options.DefaultQueueNameFormatFunc;
            DefaultPublishTopologyConfigure = options.DefaultPublishTopologyConfigure;
            DefaultReceiveEndpointConfigure = options.DefaultReceiveEndpointConfigure;

            ActiveMqPreConfigures.Clear();
            ActiveMqPreConfigures.AddRange(options.ActiveMqPreConfigures);

            ActiveMqConfigures.Clear();
            ActiveMqConfigures.AddRange(options.ActiveMqConfigures);

            ActiveMqPostConfigures.Clear();
            ActiveMqPostConfigures.AddRange(options.ActiveMqPostConfigures);

            Producers.Clear();
            Producers.AddRange(options.Producers);

            Consumers.Clear();
            Consumers.AddRange(options.Consumers);

            return this;
        }
    }
}
