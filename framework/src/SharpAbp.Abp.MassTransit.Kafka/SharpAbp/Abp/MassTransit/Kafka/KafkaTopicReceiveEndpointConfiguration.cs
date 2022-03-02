using Confluent.Kafka;

namespace SharpAbp.Abp.MassTransit.Kafka
{
    public class KafkaTopicReceiveEndpointConfiguration
    {
        /// <summary>
        /// GroupId
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// ConcurrentMessageLimit
        /// </summary>
        public int ConcurrentMessageLimit { get; set; }

        /// <summary>
        /// MaxPollInterval, default: 600000ms
        /// </summary>
        public int MaxPollInterval { get; set; } = 600000;

        /// <summary>
        /// SessionTimeout, default: 300s
        /// </summary>
        public int SessionTimeout { get; set; } = 300;

        /// <summary>
        /// EnableAutoOffsetStore
        /// </summary>
        public bool EnableAutoOffsetStore { get; set; } = true;

        /// <summary>
        /// AutoOffsetReset
        /// </summary>
        public AutoOffsetReset AutoOffsetReset { get; set; } = AutoOffsetReset.Earliest;


    }
}
