using MassTransit;
using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.MassTransit.SqlServer
{
    public class SqlServerProducerConfiguration
    {
        public IEnumerable<Type> MessageTypes { get; set; }
        public Action<ISqlMessagePublishTopologyConfigurator, Type>? Configure { get; set; }

        public SqlServerProducerConfiguration()
        {
            MessageTypes = [];
        }
    }
}
