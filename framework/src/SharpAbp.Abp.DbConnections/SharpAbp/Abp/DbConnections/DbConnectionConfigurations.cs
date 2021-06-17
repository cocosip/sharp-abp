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
                //Add default connection
                [DbConnectionNameAttribute.GetDbConnectionName<DefaultDbConnection>()] = new DbConnectionConfiguration()
            };
        }

        public DbConnectionConfigurations Configure<TConnection>(
            Action<DbConnectionConfiguration> configureAction)
        {
            return Configure(
                DbConnectionNameAttribute.GetDbConnectionName<TConnection>(),
                configureAction
            );
        }

        public DbConnectionConfigurations Configure(
            [NotNull] string name,
            [NotNull] Action<DbConnectionConfiguration> configureAction)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNull(configureAction, nameof(configureAction));

            configureAction(
                _connections.GetOrAdd(
                    name,
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
        public DbConnectionConfiguration GetConfiguration<TConnection>()
        {
            return GetConfiguration(DbConnectionNameAttribute.GetDbConnectionName<TConnection>());
        }

        [CanBeNull]
        public DbConnectionConfiguration GetConfiguration([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            return _connections.GetOrDefault(name) ??
                   Default;
        }
    }
}
