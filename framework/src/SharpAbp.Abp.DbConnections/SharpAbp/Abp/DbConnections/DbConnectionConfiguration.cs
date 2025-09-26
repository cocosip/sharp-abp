using SharpAbp.Abp.Data;

namespace SharpAbp.Abp.DbConnections
{
    /// <summary>
    /// Represents the configuration for a database connection
    /// </summary>
    public class DbConnectionConfiguration
    {
        /// <summary>
        /// Gets or sets the database provider type
        /// </summary>
        public DatabaseProvider DatabaseProvider { get; set; }

        /// <summary>
        /// Gets or sets the connection string for the database
        /// </summary>
        public string? ConnectionString { get; set; }

        /// <summary>
        /// Initializes a new instance of the DbConnectionConfiguration class
        /// </summary>
        public DbConnectionConfiguration()
        {

        }

        /// <summary>
        /// Initializes a new instance of the DbConnectionConfiguration class with specified database provider and connection string
        /// </summary>
        /// <param name="databaseProvider">The database provider type</param>
        /// <param name="connectionString">The connection string for the database</param>
        public DbConnectionConfiguration(
            DatabaseProvider databaseProvider,
            string connectionString)
        {
            DatabaseProvider = databaseProvider;
            ConnectionString = connectionString;
        }
    }
}