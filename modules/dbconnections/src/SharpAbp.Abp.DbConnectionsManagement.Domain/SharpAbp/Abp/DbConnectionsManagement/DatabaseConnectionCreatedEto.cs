using System;
using Volo.Abp.Domain.Entities.Events.Distributed;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    /// <summary>
    /// Event Transfer Object (ETO) that represents a database connection creation event
    /// </summary>
    [Serializable]
    public class DatabaseConnectionCreatedEto : EtoBase
    {
        /// <summary>
        /// Gets or sets the unique identifier of the created database connection
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Gets or sets the name of the created database connection
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the database provider type of the created connection
        /// </summary>
        public string DatabaseProvider { get; set; }
        
        /// <summary>
        /// Gets or sets the connection string of the created database connection
        /// </summary>
        public string ConnectionString { get; set; }
    }
}
