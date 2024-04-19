using MassTransit;
using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.MassTransit.PostgreSql
{
    public class PostgreSqlConsumerConfiguration
    {
        /// <summary>
        /// Configure
        /// </summary>
        public Action<IBusRegistrationConfigurator> Configure { get; set; }
        public Func<Type, bool> Filter { get; set; }
        public List<Type> Types { get; set; }
        public PostgreSqlConsumerConfiguration()
        {
            Types = [];
        }
    }
}
