namespace SharpAbp.Abp.DbConnections
{
    public class AbpDbConnectionsOptions
    {
        public DbConnectionConfigurations DbConnections { get; }

        public DatabaseProviderConfigurations DatabaseProviders { get; }

        public AbpDbConnectionsOptions()
        {
            DbConnections = new DbConnectionConfigurations();
            DatabaseProviders = new DatabaseProviderConfigurations();
        }

    }
}
