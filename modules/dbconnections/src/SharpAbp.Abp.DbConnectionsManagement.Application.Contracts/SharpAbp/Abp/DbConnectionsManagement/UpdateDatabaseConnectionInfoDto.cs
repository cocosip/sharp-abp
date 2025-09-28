using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    /// <summary>
    /// Data transfer object for updating existing database connection information.
    /// Extends ExtensibleEntityDto to support extensible properties and entity updates.
    /// Contains validation attributes to ensure data integrity during update operations.
    /// </summary>
    public class UpdateDatabaseConnectionInfoDto : ExtensibleEntityDto
    {
        /// <summary>
        /// Gets or sets the unique name identifier for the database connection.
        /// This name must be unique within the system and is used to reference the connection.
        /// </summary>
        /// <value>A string representing the connection name, must not be null or empty.</value>
        [Required]
        [DynamicStringLength(typeof(DatabaseConnectionInfoConsts), nameof(DatabaseConnectionInfoConsts.MaxNameLength))]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the database provider type for this connection.
        /// Specifies the database engine or driver to be used for establishing connections.
        /// </summary>
        /// <value>A string representing the database provider name, must not be null or empty.</value>
        [Required]
        [DynamicStringLength(typeof(DatabaseConnectionInfoConsts), nameof(DatabaseConnectionInfoConsts.MaxConnectionStringLength))]
        public string DatabaseProvider { get; set; }

        /// <summary>
        /// Gets or sets the connection string containing database connection parameters.
        /// Includes server details, database name, authentication credentials, and other connection settings.
        /// </summary>
        /// <value>A string containing the database connection configuration, can be null for default settings.</value>
        [DynamicStringLength(typeof(DatabaseConnectionInfoConsts), nameof(DatabaseConnectionInfoConsts.MaxConnectionStringLength))]
        public string ConnectionString { get; set; }
    }
}
