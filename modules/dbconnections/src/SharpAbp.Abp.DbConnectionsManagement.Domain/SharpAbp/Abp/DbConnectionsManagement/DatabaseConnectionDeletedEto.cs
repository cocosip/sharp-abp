using System;
using Volo.Abp.Domain.Entities.Events.Distributed;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    /// <summary>
    /// Event Transfer Object (ETO) that represents a database connection deletion event
    /// </summary>
    [Serializable]
    public class DatabaseConnectionDeletedEto : EtoBase
    {
        /// <summary>
        /// Gets or sets the unique identifier of the deleted database connection
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Gets or sets the name of the deleted database connection
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the database provider type of the deleted connection
        /// </summary>
        public string DatabaseProvider { get; set; }
        
        /// <summary>
        /// Gets or sets the connection string of the deleted database connection
        /// </summary>
        public string ConnectionString { get; set; }
    }
}
