using System;

namespace SharpAbp.Abp.MassTransit.ActiveMQ
{
    public static class ActiveMqUtil
    {
        public static string QueueNameFormat(string prefix, string queueName)
        {
            var name = queueName;
            if (!prefix.IsNullOrWhiteSpace())
            {
                name = $"{prefix}.{name}";
            }

            return name;
        }
    }
}
