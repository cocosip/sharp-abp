using System;
using Volo.Abp.Domain.Entities.Events.Distributed;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    /// <summary>
    /// Event Transfer Object (ETO) that represents a database connection update event
    /// </summary>
    [Serializable]
    public class DatabaseConnectionUpdatedEto : EtoBase
    {
        /// <summary>
        /// Gets or sets the unique identifier of the updated database connection
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Gets or sets the previous name of the database connection before the update
        /// </summary>
        public string OldName { get; set; }
        
        /// <summary>
        /// Gets or sets the name of the updated database connection
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the database provider type of the updated connection
        /// </summary>
        public string DatabaseProvider { get; set; }
        
        /// <summary>
        /// Gets or sets the connection string of the updated database connection
        /// </summary>
        public string ConnectionString { get; set; }
    }
}
