using System;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    /// <summary>
    /// Data transfer object representing database connection information.
    /// Extends ExtensibleAuditedEntityDto to include audit fields and extensible properties.
    /// Used for retrieving and displaying database connection details.
    /// </summary>
    public class DatabaseConnectionInfoDto : ExtensibleAuditedEntityDto<Guid>
    {
        /// <summary>
        /// Gets or sets the unique name identifier for the database connection.
        /// This name serves as a human-readable identifier for the connection configuration.
        /// </summary>
        /// <value>A string representing the connection name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the database provider type for this connection.
        /// Indicates the specific database engine or driver being used for this connection.
        /// </summary>
        /// <value>A string representing the database provider (e.g., "SqlServer", "MySQL", "PostgreSQL").</value>
        public string DatabaseProvider { get; set; }

        /// <summary>
        /// Gets or sets the connection string containing database connection parameters.
        /// Includes server information, database name, authentication details, and other connection settings.
        /// </summary>
        /// <value>A string containing the complete database connection configuration.</value>
        public string ConnectionString { get; set; }
    }
}
