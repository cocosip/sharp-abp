using System;
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


        public void Update(string name, string databaseProvider, string connectionString)
        {
            Name = name;
            DatabaseProvider = databaseProvider;
            ConnectionString = connectionString;
        }
    }
}
