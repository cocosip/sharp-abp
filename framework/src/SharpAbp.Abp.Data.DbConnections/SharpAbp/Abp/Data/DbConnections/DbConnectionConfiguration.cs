namespace SharpAbp.Abp.Data.DbConnections
{
    public class DbConnectionConfiguration
    {
        public DatabaseProvider DatabaseProvider { get; set; }

        public string ConnectionString { get; set; }

        public DbConnectionConfiguration()
        {

        }

        public DbConnectionConfiguration(
            DatabaseProvider databaseProvider,
            string connectionString)
        {
            DatabaseProvider = databaseProvider;
            ConnectionString = connectionString;
        }
    }
}
