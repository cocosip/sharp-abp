using Confluent.Kafka;
using MassTransit;
using System;

namespace SharpAbp.Abp.MassTransit.Kafka
{
    public static class KafkaUtil
    {
        public static string TopicFormat(string prefix, string topic)
        {
            if (topic.IsNullOrWhiteSpace())
            {
                return topic;
            }

            return $"{prefix}.{topic}";
        }

        public static void ConfigureReceiveEndpoint(IKafkaTopicReceiveEndpointConfigurator configurator)
        {
            configurator.MaxPollInterval = TimeSpan.FromMilliseconds(600000);
            configurator.SessionTimeout = TimeSpan.FromSeconds(300);
            configurator.ConcurrentMessageLimit = 1;
            configurator.AutoOffsetReset = AutoOffsetReset.Earliest;
            configurator.CheckpointInterval = TimeSpan.FromSeconds(20);
        }


    }
}
