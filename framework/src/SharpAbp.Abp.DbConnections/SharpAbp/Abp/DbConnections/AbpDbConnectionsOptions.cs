using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Volo.Abp.Collections;

namespace SharpAbp.Abp.DbConnections
{
    public class AbpDbConnectionsOptions
    {
        public DbConnectionConfigurations DbConnections { get; }
        public HashSet<DatabaseProvider> DatabaseProviders { get; }
        public ITypeList<IDbConnectionCreator> Creators { get; }
        public AbpDbConnectionsOptions()
        {
            DbConnections = new DbConnectionConfigurations();
            DatabaseProviders = new HashSet<DatabaseProvider>();
            Creators = new TypeList<IDbConnectionCreator>();
        }


        public AbpDbConnectionsOptions Configure(IConfiguration configuration)
        {
            var dbConnectionConfigurations = configuration
                .GetSection("DbConnectionsOptions:DbConnections")
                .Get<Dictionary<string, DbConnectionConfiguration>>();

            foreach (var dbConnectionConfigurationKv in dbConnectionConfigurations)
            {
                DbConnections.Configure(dbConnectionConfigurationKv.Key, c =>
                {
                    c.DatabaseProvider = dbConnectionConfigurationKv.Value.DatabaseProvider;
                    c.ConnectionString = dbConnectionConfigurationKv.Value.ConnectionString;
                });
            }

            var databaseProviders = configuration
                .GetSection("DbConnectionsOptions:DatabaseProviders")?
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
