using System.ComponentModel.DataAnnotations;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    /// <summary>
    /// Data transfer object for creating a new database connection information.
    /// Contains the necessary properties to establish and configure a database connection.
    /// </summary>
    public class CreateDatabaseConnectionInfoDto
    {
        /// <summary>
        /// Gets or sets the unique name identifier for the database connection.
        /// This name is used to reference the connection throughout the application.
        /// </summary>
        /// <value>A string representing the connection name, must not be null or empty.</value>
        [Required]
        [DynamicStringLength(typeof(DatabaseConnectionInfoConsts), nameof(DatabaseConnectionInfoConsts.MaxNameLength))]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the database provider type for this connection.
        /// Specifies which database engine will be used (e.g., SqlServer, MySQL, PostgreSQL, etc.).
        /// </summary>
        /// <value>A string representing the database provider name, must not be null or empty.</value>
        [Required]
        [DynamicStringLength(typeof(DatabaseConnectionInfoConsts), nameof(DatabaseConnectionInfoConsts.MaxConnectionStringLength))]
        public string DatabaseProvider { get; set; }

        /// <summary>
        /// Gets or sets the connection string used to establish the database connection.
        /// Contains all necessary parameters such as server address, database name, credentials, etc.
        /// </summary>
        /// <value>A string containing the database connection parameters, can be null for default connections.</value>
        [DynamicStringLength(typeof(DatabaseConnectionInfoConsts), nameof(DatabaseConnectionInfoConsts.MaxConnectionStringLength))]
        public string ConnectionString { get; set; }
    }
}
