using System;

namespace SharpAbp.Abp.MassTransit.RabbitMQ
{
    public static class RabbitMqUtil
    {
        public static string EntityNameFormat(string prefix, string entityName)
        {
            if (prefix.IsNullOrWhiteSpace())
            {
                return entityName;
            }

            return $"{prefix}-{entityName}";
        }
    }
}
