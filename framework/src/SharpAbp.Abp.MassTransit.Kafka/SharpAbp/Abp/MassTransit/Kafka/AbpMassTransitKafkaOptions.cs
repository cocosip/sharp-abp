using Confluent.Kafka;
using MassTransit.KafkaIntegration;
using MassTransit.KafkaIntegration.Configurators;
using MassTransit.Registration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.MassTransit.Kafka
{
    public class AbpMassTransitKafkaOptions
    {
        public string Server { get; set; }
        public bool UseSsl { get; set; }

        public string DefaultGroupId { get; set; } = "SharpAbp";

        public int DefaultConcurrentMessageLimit { get; set; } = 1;

        /// <summary>
        /// MaxPollInterval, default: 60000ms
        /// </summary>
        public int DefaultMaxPollInterval { get; set; } = 60000;

        /// <summary>
        /// SessionTimeout, default: 300s
        /// </summary>
        public int DefaultSessionTimeout { get; set; } = 300;

        /// <summary>
        /// EnableAutoOffsetStore, default: false
        /// </summary>
        public bool DefaultEnableAutoOffsetStore { get; set; } = false;

        /// <summary>
        /// AutoOffsetReset,default: AutoOffsetReset.Earliest
        /// </summary>
        public AutoOffsetReset DefaultAutoOffsetReset { get; set; } = AutoOffsetReset.Earliest;

        public Action<IKafkaSslConfigurator> ConfigureSsl { get; set; }
        public Func<string, string, string> DefaultTopicFormatFunc { get; set; }
        public Action<IKafkaTopicReceiveEndpointConfigurator> DefaultReceiveEndpointConfigure { get; set; }

        public List<Action<IRiderRegistrationConfigurator>> RiderPreConfigures { get; set; }
        public List<Action<IRiderRegistrationConfigurator>> RiderConfigures { get; set; }
        public List<Action<IRiderRegistrationConfigurator>> RiderPostConfigures { get; set; }

        public List<Action<IRiderRegistrationContext, IKafkaFactoryConfigurator>> KafkaPreConfigures { get; set; }
        public List<Action<IRiderRegistrationContext, IKafkaFactoryConfigurator>> KafkaConfigures { get; set; }
        public List<Action<IRiderRegistrationContext, IKafkaFactoryConfigurator>> KafkaPostConfigures { get; set; }

        public List<KafkaProducerConfiguration> Producers { get; set; }
        public List<KafkaConsumerConfiguration> Consumers { get; set; }

        public AbpMassTransitKafkaOptions()
        {

            RiderPreConfigures = new List<Action<IRiderRegistrationConfigurator>>();
            RiderConfigures = new List<Action<IRiderRegistrationConfigurator>>();
            RiderPostConfigures = new List<Action<IRiderRegistrationConfigurator>>();

            KafkaPreConfigures = new List<Action<IRiderRegistrationContext, IKafkaFactoryConfigurator>>();
            KafkaConfigures = new List<Action<IRiderRegistrationContext, IKafkaFactoryConfigurator>>();
            KafkaPostConfigures = new List<Action<IRiderRegistrationContext, IKafkaFactoryConfigurator>>();

            Producers = new List<KafkaProducerConfiguration>();
            Consumers = new List<KafkaConsumerConfiguration>();
        }

        public AbpMassTransitKafkaOptions PreConfigure(IConfiguration configuration)
        {
            var massTransitKafkaOptions = configuration
                .GetSection("MassTransitOptions:KafkaOptions")
                .Get<AbpMassTransitKafkaOptions>();
            if (massTransitKafkaOptions != null)
            {
                Server = massTransitKafkaOptions.Server;
                UseSsl = massTransitKafkaOptions.UseSsl;

                DefaultGroupId = massTransitKafkaOptions.DefaultGroupId;
                DefaultConcurrentMessageLimit = massTransitKafkaOptions.DefaultConcurrentMessageLimit;
                DefaultMaxPollInterval = massTransitKafkaOptions.DefaultMaxPollInterval;
                DefaultSessionTimeout = massTransitKafkaOptions.DefaultSessionTimeout;
                DefaultEnableAutoOffsetStore = massTransitKafkaOptions.DefaultEnableAutoOffsetStore;
                DefaultAutoOffsetReset = massTransitKafkaOptions.DefaultAutoOffsetReset;
            }

            return this;
        }


    }
}
