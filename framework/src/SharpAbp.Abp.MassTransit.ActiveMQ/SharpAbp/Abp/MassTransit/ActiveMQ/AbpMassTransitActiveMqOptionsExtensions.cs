using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpAbp.Abp.MassTransit.ActiveMQ
{
    public static class AbpMassTransitActiveMqOptionsExtensions
    {
        public static List<(string, Action<string, IActiveMqBusFactoryConfigurator>)> GetMessageConfigures(this AbpMassTransitActiveMqOptions options)
        {
            var messageConfigures = new List<(string, Action<string, IActiveMqBusFactoryConfigurator>)>();

            foreach (var consumer in options.Consumers)
            {
                if (!messageConfigures.Any(x => x.Item1 == consumer.QueueName))
                {
                    messageConfigures.Add((consumer.QueueName!, consumer.MessageConfigure!));
                }
            }

            foreach (var producer in options.Producers)
            {
                if (!messageConfigures.Any(x => x.Item1 == producer.QueueName))
                {
                    messageConfigures.Add((producer.QueueName!, producer.MessageConfigure!));
                }
            }

            return messageConfigures;
        }
    }
}
