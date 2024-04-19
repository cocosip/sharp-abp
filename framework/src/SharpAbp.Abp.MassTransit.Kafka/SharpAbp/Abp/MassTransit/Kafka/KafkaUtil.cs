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
    }
}
