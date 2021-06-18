using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Volo.Abp;

namespace SharpAbp.Abp.DbConnections
{
    public class DbConnectionConfigurations
    {
        private DbConnectionConfiguration Default => GetConfiguration<DefaultDbConnection>();

        private readonly Dictionary<string, DbConnectionConfiguration> _connections;

        public DbConnectionConfigurations()
        {
            _connections = new Dictionary<string, DbConnectionConfiguration>
            {
                [DbConnectionNameAttribute.GetDbConnectionName<DefaultDbConnection>()] = new DbConnectionConfiguration(DatabaseProvider.InMemory, "")
            };
        }

        public DbConnectionConfigurations Configure<TDbConnection>(
            Action<DbConnectionConfiguration> configureAction)
        {
            return Configure(
                DbConnectionNameAttribute.GetDbConnectionName<TDbConnection>(),
                configureAction
            );
        }

        public DbConnectionConfigurations Configure(
            [NotNull] string dbConnectionName,
            [NotNull] Action<DbConnectionConfiguration> configureAction)
        {
            Check.NotNullOrWhiteSpace(dbConnectionName, nameof(dbConnectionName));
            Check.NotNull(configureAction, nameof(configureAction));

            configureAction(
                _connections.GetOrAdd(
                    dbConnectionName,
                    () => new DbConnectionConfiguration()
                )
            );

            return this;
        }

        public DbConnectionConfigurations ConfigureDefault(Action<DbConnectionConfiguration> configureAction)
        {
            configureAction(Default);
            return this;
        }

        public DbConnectionConfigurations ConfigureAll(Action<string, DbConnectionConfiguration> configureAction)
        {
            foreach (var connectionKv in _connections)
            {
                configureAction(connectionKv.Key, connectionKv.Value);
            }

            return this;
        }

        [CanBeNull]
        public DbConnectionConfiguration GetConfiguration<TDbConnection>()
        {
            return GetConfiguration(DbConnectionNameAttribute.GetDbConnectionName<TDbConnection>());
        }

        [CanBeNull]
        public DbConnectionConfiguration GetConfiguration([NotNull] string dbConnectionName)
        {
            Check.NotNullOrWhiteSpace(dbConnectionName, nameof(dbConnectionName));
            return _connections.GetOrDefault(dbConnectionName) ??
                   Default;
        }
    }
}
