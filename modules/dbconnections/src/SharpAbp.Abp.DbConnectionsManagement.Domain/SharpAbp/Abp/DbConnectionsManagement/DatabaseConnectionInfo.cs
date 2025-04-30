using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    public class DatabaseConnectionInfo : AuditedAggregateRoot<Guid>
    {
        /// <summary>
        /// DbConnection name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Database Provider
        /// </summary>
        public virtual string DatabaseProvider { get; set; }

        /// <summary>
        /// Database connection string
        /// </summary>
        public virtual string ConnectionString { get; set; }

        public DatabaseConnectionInfo()
        {

        }

        public DatabaseConnectionInfo(Guid id, string name, string databaseProvider, string connectionString)
        {
            Id = id;
            Name = name;
            DatabaseProvider = databaseProvider;
            ConnectionString = connectionString;
        }

        public virtual void Update(string databaseProvider, string connectionString)
        {
            Check.NotNullOrWhiteSpace(databaseProvider, nameof(databaseProvider));
            Check.NotNullOrWhiteSpace(connectionString, nameof(connectionString));

            DatabaseProvider = databaseProvider;
            ConnectionString = connectionString;

            AddDistributedEvent(new DatabaseConnectionUpdatedEto
            {
                Id = Id,
                Name = Name,
                DatabaseProvider = DatabaseProvider,
                ConnectionString = ConnectionString
            });
        }

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
