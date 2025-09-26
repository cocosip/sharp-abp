using Microsoft.Extensions.Configuration;
using SharpAbp.Abp.Data;
using System.Collections.Generic;
using Volo.Abp.Collections;

namespace SharpAbp.Abp.DbConnections
{
    /// <summary>
    /// Represents the options for configuring database connections in the ABP framework
    /// </summary>
    public class AbpDbConnectionsOptions
    {
        /// <summary>
        /// Gets the collection of database connection configurations
        /// </summary>
        public DbConnectionConfigurations DbConnections { get; }

        /// <summary>
        /// Gets the set of supported database providers
        /// </summary>
        public HashSet<DatabaseProvider> DatabaseProviders { get; }

        /// <summary>
        /// Gets the list of database connection creators
        /// </summary>
        public ITypeList<IDbConnectionCreator> Creators { get; }

        /// <summary>
        /// Initializes a new instance of the AbpDbConnectionsOptions class
        /// </summary>
        public AbpDbConnectionsOptions()
        {
            DbConnections = new DbConnectionConfigurations();
            DatabaseProviders = [];
            Creators = new TypeList<IDbConnectionCreator>();
        }

        /// <summary>
        /// Configures the database connections options from the provided configuration section
        /// </summary>
        /// <param name="configuration">The configuration section containing database connection settings</param>
        /// <returns>The current AbpDbConnectionsOptions instance for method chaining</returns>
        public AbpDbConnectionsOptions Configure(IConfiguration configuration)
        {
            var dbConnectionConfigurations = configuration
                .GetSection("DbConnectionsOptions:DbConnections")
                .Get<Dictionary<string, DbConnectionConfiguration>>();

            if (dbConnectionConfigurations != null)
            {
                foreach (var dbConnectionConfigurationKv in dbConnectionConfigurations)
                {
                    DbConnections.Configure(dbConnectionConfigurationKv.Key, c =>
                    {
                        c.DatabaseProvider = dbConnectionConfigurationKv.Value.DatabaseProvider;
                        c.ConnectionString = dbConnectionConfigurationKv.Value.ConnectionString;
                    });
                }
            }

            var databaseProviders = configuration
                .GetSection("DbConnectionsOptions:DatabaseProviders")
                .Get<List<DatabaseProvider>>();
            if (databaseProviders != null)
            {
                foreach (var databaseProvider in databaseProviders)
                {
                    DatabaseProviders.AddIfNotContains(databaseProvider);
                }
            }

            return this;
        }
    }
}