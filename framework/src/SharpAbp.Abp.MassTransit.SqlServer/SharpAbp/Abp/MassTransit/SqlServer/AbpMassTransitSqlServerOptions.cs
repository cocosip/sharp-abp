using MassTransit;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.MassTransit.SqlServer
{
    public class AbpMassTransitSqlServerOptions
    {
        public string? ConnectionString { get; set; }
        public bool Create { get; set; } = true;
        public bool Delete { get; set; } = false;
        public TimeSpan? AutoDeleteOnIdle { get; set; } = TimeSpan.FromSeconds(600);
        public bool AutoStart { get; set; } = true;

        public int ConcurrentMessageLimit { get; set; } = 1;
        public int PrefetchCount { get; set; } = 4;

        public Action<ISqlMessagePublishTopologyConfigurator, Type>? DefaultPublishTopologyConfigurator { get; set; }
        public Func<Type, bool>? DefaultFilter { get; set; }

        public SqlTransportOptions? SqlTransportOptions { get; set; }

        public Action<SqlTransportOptions>? SqlTransportConfigure { get; set; }
        public List<Action<IBusRegistrationContext, ISqlBusFactoryConfigurator>>? SqlServerPreConfigures { get; set; }
        public List<Action<IBusRegistrationContext, ISqlBusFactoryConfigurator>>? SqlServerConfigures { get; set; }
        public List<Action<IBusRegistrationContext, ISqlBusFactoryConfigurator>>? SqlServerPostConfigures { get; set; }


        public List<SqlServerProducerConfiguration>? Producers { get; set; }
        public List<SqlServerConsumerConfiguration>? Consumers { get; set; }
        public AbpMassTransitSqlServerOptions PreConfigure(IConfiguration configuration)
        {
            SqlServerPreConfigures = [];
            SqlServerConfigures = [];
            SqlServerPostConfigures = [];
            SqlTransportOptions = new SqlTransportOptions();

            Producers = [];
            Consumers = [];

            var massTransitPostgreSqlOptions = configuration
                .GetSection("MassTransitOptions:SqlServerOptions")
                .Get<AbpMassTransitSqlServerOptions>();

            if (massTransitPostgreSqlOptions != null)
            {
                ConnectionString = massTransitPostgreSqlOptions.ConnectionString;
                Create = massTransitPostgreSqlOptions.Create;
                Delete = massTransitPostgreSqlOptions.Delete;
                AutoDeleteOnIdle = massTransitPostgreSqlOptions.AutoDeleteOnIdle;

                ConcurrentMessageLimit = massTransitPostgreSqlOptions.ConcurrentMessageLimit;
                PrefetchCount = massTransitPostgreSqlOptions.PrefetchCount;

                if (massTransitPostgreSqlOptions.SqlTransportOptions != null)
                {
                    SqlTransportOptions = new SqlTransportOptions()
                    {
                        Host = massTransitPostgreSqlOptions.SqlTransportOptions.Host,
                        Port = massTransitPostgreSqlOptions.SqlTransportOptions.Port,
                        Database = massTransitPostgreSqlOptions.SqlTransportOptions.Database,
                        Schema = massTransitPostgreSqlOptions.SqlTransportOptions.Schema,
                        Role = massTransitPostgreSqlOptions.SqlTransportOptions.Role,
                        Username = massTransitPostgreSqlOptions.SqlTransportOptions.Username,
                        Password = massTransitPostgreSqlOptions.SqlTransportOptions.Password,
                        AdminUsername = massTransitPostgreSqlOptions.SqlTransportOptions.AdminUsername,
                        AdminPassword = massTransitPostgreSqlOptions.SqlTransportOptions.AdminPassword,
                    };
                }
            }

            return this;
        }
    }
}
