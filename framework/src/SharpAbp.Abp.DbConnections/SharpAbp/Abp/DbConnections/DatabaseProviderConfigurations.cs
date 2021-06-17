using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Volo.Abp;

namespace SharpAbp.Abp.DbConnections
{
    public class DatabaseProviderConfigurations
    {
        private readonly Dictionary<DatabaseProvider, DatabaseProviderConfiguration> _databaseProviders;

        public DatabaseProviderConfigurations()
        {
            _databaseProviders = new Dictionary<DatabaseProvider, DatabaseProviderConfiguration>();
        }

        public DatabaseProviderConfigurations Configure(
            [NotNull] DatabaseProvider databaseProvider,
            [NotNull] Action<DatabaseProviderConfiguration> configureAction)
        {
            Check.NotNull(databaseProvider, nameof(databaseProvider));
            Check.NotNull(configureAction, nameof(configureAction));

            configureAction(
                _databaseProviders.GetOrAdd(
                    databaseProvider,
                    () => new DatabaseProviderConfiguration()
                )
            );

            return this;
        }

        public DatabaseProviderConfigurations ConfigureAll(Action<DatabaseProvider, DatabaseProviderConfiguration> configureAction)
        {
            foreach (var databaseProviderKv in _databaseProviders)
            {
                configureAction(databaseProviderKv.Key, databaseProviderKv.Value);
            }

            return this;
        }


        [NotNull]
        public DatabaseProviderConfiguration GetConfiguration([NotNull] DatabaseProvider databaseProvider)
        {
            Check.NotNull(databaseProvider, nameof(databaseProvider));
            return _databaseProviders.GetOrDefault(databaseProvider);
        }

    }
}
