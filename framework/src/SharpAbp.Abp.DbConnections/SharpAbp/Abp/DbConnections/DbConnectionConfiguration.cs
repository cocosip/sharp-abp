using SharpAbp.Abp.Data;

namespace SharpAbp.Abp.DbConnections
{
    public class DbConnectionConfiguration
    {
        public DatabaseProvider DatabaseProvider { get; set; }

        public string? ConnectionString { get; set; }

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
