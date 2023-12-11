using Confluent.Kafka;
using MassTransit;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.MassTransit.Kafka
{
    public class AbpMassTransitKafkaOptions
    {
        public string Server { get; set; }

        /// <summary>
        /// UseSSL
        /// </summary>
        public bool UseSSL { get; set; }

        /// <summary>
        /// GroupId, default: SharpAbp
        /// </summary>
        public string DefaultGroupId { get; set; } = "SharpAbp";

        /// <summary>
        /// ClientId, default: rdkafka
        /// </summary>
        public string DefaultClientId { get; set; } = "rdkafka";

        /// <summary>
        /// ConcurrentMessageLimit, default: 1
        /// </summary>
        public int DefaultConcurrentMessageLimit { get; set; } = 1;

        /// <summary>
        /// AutoCreateTopic, default: true
        /// Only consumer will auto create
        /// </summary>
        public bool AutoCreateTopic { get; set; } = true;

        /// <summary>
        /// NumPartitions, default: 3
        /// </summary>
        public ushort DefaultNumPartitions { get; set; } = 3;

        /// <summary>
        /// ReplicationFactor, default: 3
        /// </summary>
        public short DefaultReplicationFactor { get; set; } = 3;

        /// <summary>
        /// ConcurrentConsumerLimit, default: 1
        /// </summary>
        public ushort DefaultConcurrentConsumerLimit { get; set; } = 1;

        /// <summary>
        /// MessageLimit, default: 1000
        /// </summary>
        public ushort DefaultMessageLimit { get; set; } = 1000;

        /// <summary>
        /// PrefetchCount, default: 500
        /// </summary>
        public ushort DefaultPrefetchCount { get; set; } = 500;

        /// <summary>
        /// MaxPollInterval, default: 60000ms
        /// </summary>
        public TimeSpan DefaultMaxPollInterval { get; set; } = TimeSpan.FromMilliseconds(60000);

        /// <summary>
        /// SessionTimeout, default: 300s
        /// </summary>
        public TimeSpan DefaultSessionTimeout { get; set; } = TimeSpan.FromSeconds(300);

        /// <summary>
        /// CheckpointInterval, default: 20s
        /// </summary>
        public TimeSpan DefaultCheckpointInterval { get; set; } = TimeSpan.FromSeconds(20);

        /// <summary>
        /// CheckpointMessageCount, default: 1000
        /// </summary>
        public ushort DefaultCheckpointMessageCount { get; set; } = 1000;

        /// <summary>
        /// EnableAutoOffsetStore, default: false
        /// </summary>
        public bool DefaultEnableAutoOffsetStore { get; set; } = false;

        /// <summary>
        /// AutoOffsetReset,default: AutoOffsetReset.Earliest
        /// </summary>
        public AutoOffsetReset DefaultAutoOffsetReset { get; set; } = AutoOffsetReset.Earliest;

        /// <summary>
        /// HeartbeatInterval, default: 20s
        /// </summary>
        public TimeSpan DefaultHeartbeatInterval { get; set; } = TimeSpan.FromSeconds(20);

        /// <summary>
        /// The maximum time to wait before reconnecting to a broker after the connection has been closed. default: 10000 importance: medium
        /// </summary>
        public TimeSpan DefaultReconnectBackoff { get; set; } = TimeSpan.FromMilliseconds(10000);

        /// <summary>
        /// The initial time to wait before reconnecting to a broker after the connection has been closed. The time is increased exponentially until `reconnect.backoff.max.ms` is reached. -25% to +50% jitter is applied to each reconnect backoff. A value of 0 disables the backoff and reconnects immediately. default: 100 importance: medium
        /// </summary>
        public TimeSpan DefaultReconnectBackoffMax { get; set; } = TimeSpan.FromMilliseconds(100);

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

            RiderPreConfigures = [];
            RiderConfigures = [];
            RiderPostConfigures = [];

            KafkaPreConfigures = [];
            KafkaConfigures = [];
            KafkaPostConfigures = [];

            Producers = [];
            Consumers = [];
        }

        public AbpMassTransitKafkaOptions PreConfigure(IConfiguration configuration)
        {
            var massTransitKafkaOptions = configuration
                .GetSection("MassTransitOptions:KafkaOptions")
                .Get<AbpMassTransitKafkaOptions>();
            if (massTransitKafkaOptions != null)
            {
                Server = massTransitKafkaOptions.Server;
                UseSSL = massTransitKafkaOptions.UseSSL;

                DefaultGroupId = massTransitKafkaOptions.DefaultGroupId;
                DefaultClientId = massTransitKafkaOptions.DefaultClientId;
                DefaultConcurrentMessageLimit = massTransitKafkaOptions.DefaultConcurrentMessageLimit;
                AutoCreateTopic = massTransitKafkaOptions.AutoCreateTopic;
                DefaultNumPartitions = massTransitKafkaOptions.DefaultNumPartitions;
                DefaultReplicationFactor = massTransitKafkaOptions.DefaultReplicationFactor;
                DefaultConcurrentConsumerLimit = massTransitKafkaOptions.DefaultConcurrentConsumerLimit;
                DefaultMessageLimit = massTransitKafkaOptions.DefaultMessageLimit;
                DefaultPrefetchCount = massTransitKafkaOptions.DefaultPrefetchCount;
                DefaultMaxPollInterval = massTransitKafkaOptions.DefaultMaxPollInterval;
                DefaultCheckpointInterval = massTransitKafkaOptions.DefaultCheckpointInterval;
                DefaultCheckpointMessageCount = massTransitKafkaOptions.DefaultCheckpointMessageCount;
                DefaultSessionTimeout = massTransitKafkaOptions.DefaultSessionTimeout;
                DefaultEnableAutoOffsetStore = massTransitKafkaOptions.DefaultEnableAutoOffsetStore;
                DefaultAutoOffsetReset = massTransitKafkaOptions.DefaultAutoOffsetReset;
                DefaultHeartbeatInterval = massTransitKafkaOptions.DefaultHeartbeatInterval;
                DefaultReconnectBackoff = massTransitKafkaOptions.DefaultReconnectBackoff;
                DefaultReconnectBackoffMax = massTransitKafkaOptions.DefaultReconnectBackoffMax;
            }

            return this;
        }


    }
}
