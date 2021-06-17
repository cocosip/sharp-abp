using Volo.Abp.Collections;

namespace SharpAbp.Abp.DbConnections
{
    public class DbConnectionConfiguration
    {
        /// <summary>
        /// Database provider
        /// </summary>
        public DatabaseProvider DatabaseProvider { get; set; }

        /// <summary>
        /// Database connection string
        /// </summary>
        public string ConnectionString { get; set; }

        public DbConnectionConfiguration()
        {

        }

        public DbConnectionConfiguration(
            DatabaseProvider databaseProvider,
            string connectionString) : this()
        {
            DatabaseProvider = databaseProvider;
            ConnectionString = connectionString;
        }
    }
}
