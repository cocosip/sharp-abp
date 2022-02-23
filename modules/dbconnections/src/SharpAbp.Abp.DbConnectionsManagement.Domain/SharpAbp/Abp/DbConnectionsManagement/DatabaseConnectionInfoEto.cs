using System;
using Volo.Abp.Domain.Entities.Events.Distributed;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    public class DatabaseConnectionInfoEto : EntityEto
    {
        public Guid Id { get; set; }

        /// <summary>
        /// DbConnection name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Database Provider
        /// </summary>
        public string DatabaseProvider { get; set; }

        /// <summary>
        /// Database connection string
        /// </summary>
        public string ConnectionString { get; set; }
    }
}
