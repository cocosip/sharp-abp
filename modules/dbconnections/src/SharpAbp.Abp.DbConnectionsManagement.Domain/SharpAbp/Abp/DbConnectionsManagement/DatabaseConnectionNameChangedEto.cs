using System;
using Volo.Abp.Domain.Entities.Events.Distributed;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    /// <summary>
    /// Event Transfer Object (ETO) that represents a database connection name change event
    /// </summary>
    [Serializable]
    public class DatabaseConnectionNameChangedEto : EtoBase
    {
        /// <summary>
        /// Gets or sets the unique identifier of the database connection whose name was changed
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Gets or sets the previous name of the database connection before the change
        /// </summary>
        public string OldName { get; set; }
        
        /// <summary>
        /// Gets or sets the new name of the database connection after the change
        /// </summary>
        public string Name { get; set; }
    }
}
