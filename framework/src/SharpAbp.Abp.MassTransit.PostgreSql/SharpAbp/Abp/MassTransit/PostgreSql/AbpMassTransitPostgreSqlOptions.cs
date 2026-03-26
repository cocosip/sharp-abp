using MassTransit;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.MassTransit.PostgreSql
{
    public class AbpMassTransitPostgreSqlOptions
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
        public List<Action<IBusRegistrationContext, ISqlBusFactoryConfigurator>>? PostgreSqlPreConfigures { get; set; }
        public List<Action<IBusRegistrationContext, ISqlBusFactoryConfigurator>>? PostgreSqlConfigures { get; set; }
        public List<Action<IBusRegistrationContext, ISqlBusFactoryConfigurator>>? PostgreSqlPostConfigures { get; set; }


        public List<PostgreSqlProducerConfiguration>? Producers { get; set; }
        public List<PostgreSqlConsumerConfiguration>? Consumers { get; set; }
        public AbpMassTransitPostgreSqlOptions PreConfigure(IConfiguration configuration)
        {
            PostgreSqlPreConfigures = [];
            PostgreSqlConfigures = [];
            PostgreSqlPostConfigures = [];
            SqlTransportOptions = new SqlTransportOptions();

            Producers = [];
            Consumers = [];

            var massTransitPostgreSqlOptions = configuration
                .GetSection("MassTransitOptions:PostgreSqlOptions")
                .Get<AbpMassTransitPostgreSqlOptions>();

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

        public AbpMassTransitPostgreSqlOptions CopyFrom(AbpMassTransitPostgreSqlOptions options)
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

            PostgreSqlPreConfigures ??= [];
            PostgreSqlPreConfigures.Clear();
            PostgreSqlPreConfigures.AddRange(options.PostgreSqlPreConfigures ?? []);

            PostgreSqlConfigures ??= [];
            PostgreSqlConfigures.Clear();
            PostgreSqlConfigures.AddRange(options.PostgreSqlConfigures ?? []);

            PostgreSqlPostConfigures ??= [];
            PostgreSqlPostConfigures.Clear();
            PostgreSqlPostConfigures.AddRange(options.PostgreSqlPostConfigures ?? []);

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
