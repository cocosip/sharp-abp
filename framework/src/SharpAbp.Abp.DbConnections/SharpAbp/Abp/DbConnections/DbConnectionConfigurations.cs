﻿using JetBrains.Annotations;
using SharpAbp.Abp.Data;
using System;
using System.Collections.Generic;
using Volo.Abp;

namespace SharpAbp.Abp.DbConnections
{
    /// <summary>
    /// Manages a collection of database connection configurations
    /// </summary>
    public class DbConnectionConfigurations
    {
        /// <summary>
        /// Gets the default database connection configuration
        /// </summary>
        private DbConnectionConfiguration Default => GetConfiguration<DefaultDbConnection>();

        private readonly Dictionary<string, DbConnectionConfiguration> _connections;

        /// <summary>
        /// Initializes a new instance of the DbConnectionConfigurations class with a default in-memory database connection
        /// </summary>
        public DbConnectionConfigurations()
        {
            _connections = new Dictionary<string, DbConnectionConfiguration>
            {
                [DbConnectionNameAttribute.GetDbConnectionName<DefaultDbConnection>()] = new DbConnectionConfiguration(DatabaseProvider.InMemory, "")
            };
        }

        /// <summary>
        /// Configures a database connection for the specified database connection type
        /// </summary>
        /// <typeparam name="TDbConnection">The type of the database connection</typeparam>
        /// <param name="configureAction">The action to configure the database connection</param>
        /// <returns>The current DbConnectionConfigurations instance for method chaining</returns>
        public DbConnectionConfigurations Configure<TDbConnection>(
            Action<DbConnectionConfiguration> configureAction)
        {
            return Configure(
                DbConnectionNameAttribute.GetDbConnectionName<TDbConnection>(),
                configureAction
            );
        }

        /// <summary>
        /// Configures a database connection with the specified name
        /// </summary>
        /// <param name="dbConnectionName">The name of the database connection to configure</param>
        /// <param name="configureAction">The action to configure the database connection</param>
        /// <returns>The current DbConnectionConfigurations instance for method chaining</returns>
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

        /// <summary>
        /// Configures the default database connection
        /// </summary>
        /// <param name="configureAction">The action to configure the default database connection</param>
        /// <returns>The current DbConnectionConfigurations instance for method chaining</returns>
        public DbConnectionConfigurations ConfigureDefault(Action<DbConnectionConfiguration> configureAction)
        {
            configureAction(Default);
            return this;
        }

        /// <summary>
        /// Configures all database connections using the specified action
        /// </summary>
        /// <param name="configureAction">The action to configure each database connection</param>
        /// <returns>The current DbConnectionConfigurations instance for method chaining</returns>
        public DbConnectionConfigurations ConfigureAll(Action<string, DbConnectionConfiguration> configureAction)
        {
            foreach (var connectionKv in _connections)
            {
                configureAction(connectionKv.Key, connectionKv.Value);
            }

            return this;
        }

        /// <summary>
        /// Gets the database connection configuration for the specified database connection type
        /// </summary>
        /// <typeparam name="TDbConnection">The type of the database connection</typeparam>
        /// <returns>The database connection configuration, or null if not found</returns>
        [CanBeNull]
        public DbConnectionConfiguration GetConfiguration<TDbConnection>()
        {
            return GetConfiguration(DbConnectionNameAttribute.GetDbConnectionName<TDbConnection>());
        }

        /// <summary>
        /// Gets the database connection configuration with the specified name
        /// </summary>
        /// <param name="dbConnectionName">The name of the database connection to retrieve</param>
        /// <returns>The database connection configuration, or null if not found</returns>
        [CanBeNull]
        public DbConnectionConfiguration GetConfiguration([NotNull] string dbConnectionName)
        {
            Check.NotNullOrWhiteSpace(dbConnectionName, nameof(dbConnectionName));
            return _connections.GetOrDefault(dbConnectionName) ??
                   Default;
        }
    }
}