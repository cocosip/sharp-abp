using MassTransit;
using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.MassTransit.SqlServer
{
    public class SqlServerConsumerConfiguration
    {
        /// <summary>
        /// Configure
        /// </summary>
        public Action<IBusRegistrationConfigurator> Configure { get; set; }
        public Func<Type, bool> Filter { get; set; }
        public List<Type> Types { get; set; }
        public SqlServerConsumerConfiguration()
        {
            Types = [];
        }
    }
}
