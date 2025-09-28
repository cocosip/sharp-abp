using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    /// <summary>
    /// Represents a database connection information aggregate root that manages database connection details
    /// </summary>
    public class DatabaseConnectionInfo : AuditedAggregateRoot<Guid>
    {
        /// <summary>
        /// Gets or sets the unique name of the database connection
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the database provider type (e.g., SqlServer, MySQL, PostgreSQL)
        /// </summary>
        public virtual string DatabaseProvider { get; set; }

        /// <summary>
        /// Gets or sets the database connection string used to establish connection
        /// </summary>
        public virtual string ConnectionString { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseConnectionInfo"/> class
        /// </summary>
        public DatabaseConnectionInfo()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseConnectionInfo"/> class with specified parameters
        /// </summary>
        /// <param name="id">The unique identifier for the database connection</param>
        /// <param name="name">The name of the database connection</param>
        /// <param name="databaseProvider">The database provider type</param>
        /// <param name="connectionString">The connection string for the database</param>
        public DatabaseConnectionInfo(Guid id, string name, string databaseProvider, string connectionString) : base(id)
        {
            Name = name;
            DatabaseProvider = databaseProvider;
            ConnectionString = connectionString;
        }

        /// <summary>
        /// Updates the database connection information with new values and publishes an update event
        /// </summary>
        /// <param name="name">The new name for the database connection</param>
        /// <param name="databaseProvider">The new database provider type</param>
        /// <param name="connectionString">The new connection string</param>
        public virtual void Update(string name, string databaseProvider, string connectionString)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNullOrWhiteSpace(databaseProvider, nameof(databaseProvider));
            Check.NotNullOrWhiteSpace(connectionString, nameof(connectionString));

            var oldName = Name;
            Name = name;
            DatabaseProvider = databaseProvider;
            ConnectionString = connectionString;

            AddDistributedEvent(new DatabaseConnectionUpdatedEto
            {
                Id = Id,
                OldName = oldName,
                Name = name,
                DatabaseProvider = databaseProvider,
                ConnectionString = connectionString
            });
        }

        /// <summary>
        /// Changes the name of the database connection and publishes a name change event
        /// </summary>
        /// <param name="name">The new name for the database connection</param>
        public virtual void ChangeName(string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            var oldName = Name;
            Name = name;

            AddDistributedEvent(new DatabaseConnectionNameChangedEto
            {
                Id = Id,
                Name = Name,
                OldName = oldName,
            });
        }
    }
}
