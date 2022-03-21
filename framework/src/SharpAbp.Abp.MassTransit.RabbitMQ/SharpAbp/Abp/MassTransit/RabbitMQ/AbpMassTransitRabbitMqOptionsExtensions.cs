using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpAbp.Abp.MassTransit.RabbitMQ
{
    public static class AbpMassTransitRabbitMqOptionsExtensions
    {
        public static List<(string, Action<string, IRabbitMqBusFactoryConfigurator>)> GetMessageConfigures(this AbpMassTransitRabbitMqOptions options)
        {
            var messageConfigures = new List<(string, Action<string, IRabbitMqBusFactoryConfigurator>)>();

            foreach (var consumer in options.Consumers)
            {
                if (!messageConfigures.Any(x => x.Item1 == consumer.ExchangeName))
                {
                    messageConfigures.Add((consumer.ExchangeName, consumer.MessageConfigure));
                }
            }

            foreach (var producer in options.Producers)
            {
                if (!messageConfigures.Any(x => x.Item1 == producer.ExchangeName))
                {
                    messageConfigures.Add((producer.ExchangeName, producer.MessageConfigure));
                }
            }

            return messageConfigures;
        }
    }
}
