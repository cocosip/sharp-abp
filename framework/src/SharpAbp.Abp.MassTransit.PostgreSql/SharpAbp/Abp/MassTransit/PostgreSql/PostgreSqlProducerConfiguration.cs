using MassTransit;
using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.MassTransit.PostgreSql
{
    public class PostgreSqlProducerConfiguration
    {
        public IEnumerable<Type> MessageTypes { get; set; }
        public Action<ISqlMessagePublishTopologyConfigurator, Type> Configure { get; set; }

        public PostgreSqlProducerConfiguration()
        {
            MessageTypes = [];
        }
    }
}
