using System;

namespace SharpAbp.Abp.MassTransit.RabbitMQ
{
    public static class RabbitMqUtil
    {
        public static string ExchangeNameFormat(string prefix, string exchangeName)
        {
            if (prefix.IsNullOrWhiteSpace())
            {
                return exchangeName;
            }

            return $"{prefix}.{exchangeName}";
        }

        public static string QueueNameFormat(string prefix, string queuePrefix, string queueName)
        {
            //Prefix-QueuePrefix-QueueName
            var name = queueName;
            if (!queuePrefix.IsNullOrWhiteSpace())
            {
                name = $"{queuePrefix}.{name}";
            }

            if (!prefix.IsNullOrWhiteSpace())
            {
                name = $"{prefix}.{name}";
            }

            return name;
        }
    }
}
