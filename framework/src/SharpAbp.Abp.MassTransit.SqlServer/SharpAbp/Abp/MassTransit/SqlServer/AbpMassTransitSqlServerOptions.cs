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

        public AbpMassTransitSqlServerOptions CopyFrom(AbpMassTransitSqlServerOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            ConnectionString = options.ConnectionString;
            Create = options.Create;
            Delete = options.Delete;
            AutoDeleteOnIdle = options.AutoDeleteOnIdle;
            AutoStart = options.AutoStart;
            ConcurrentMessageLimit = options.ConcurrentMessageLimit;
            PrefetchCount = options.PrefetchCount;
            DefaultPublishTopologyConfigurator = options.DefaultPublishTopologyConfigurator;
            DefaultFilter = options.DefaultFilter;
            SqlTransportConfigure = options.SqlTransportConfigure;

            SqlTransportOptions = options.SqlTransportOptions == null
                ? null
                : new SqlTransportOptions
                {
                    Host = options.SqlTransportOptions.Host,
                    Port = options.SqlTransportOptions.Port,
                    Database = options.SqlTransportOptions.Database,
                    Schema = options.SqlTransportOptions.Schema,
                    Role = options.SqlTransportOptions.Role,
                    Username = options.SqlTransportOptions.Username,
                    Password = options.SqlTransportOptions.Password,
                    AdminUsername = options.SqlTransportOptions.AdminUsername,
                    AdminPassword = options.SqlTransportOptions.AdminPassword
                };

            SqlServerPreConfigures ??= [];
            SqlServerPreConfigures.Clear();
            SqlServerPreConfigures.AddRange(options.SqlServerPreConfigures ?? []);

            SqlServerConfigures ??= [];
            SqlServerConfigures.Clear();
            SqlServerConfigures.AddRange(options.SqlServerConfigures ?? []);

            SqlServerPostConfigures ??= [];
            SqlServerPostConfigures.Clear();
            SqlServerPostConfigures.AddRange(options.SqlServerPostConfigures ?? []);

            Producers ??= [];
            Producers.Clear();
            Producers.AddRange(options.Producers ?? []);

            Consumers ??= [];
            Consumers.Clear();
            Consumers.AddRange(options.Consumers ?? []);

            return this;
        }
    }
}
