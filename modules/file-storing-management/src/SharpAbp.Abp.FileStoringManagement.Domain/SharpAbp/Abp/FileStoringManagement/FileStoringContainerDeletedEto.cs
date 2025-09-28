﻿using System;
using Volo.Abp.Domain.Entities.Events.Distributed;

namespace SharpAbp.Abp.FileStoringManagement
{
    /// <summary>
    /// Represents an event transfer object that is published when a file storing container is deleted.
    /// This event carries information about container deletion and is used for distributed communication
    /// between services to maintain consistency and trigger cleanup operations across the system.
    /// </summary>
    [Serializable]
    public class FileStoringContainerDeletedEto : EtoBase
    {
        /// <summary>
        /// Gets or sets the unique identifier of the deleted file storing container.
        /// </summary>
        /// <value>The container's unique identifier.</value>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Gets or sets the tenant identifier that owned this container.
        /// Null indicates this container belonged to the host.
        /// </summary>
        /// <value>The tenant identifier, or null for host-owned containers.</value>
        public Guid? TenantId { get; set; }
        
        /// <summary>
        /// Gets or sets the name of the deleted container.
        /// This is used for cleanup operations and logging purposes.
        /// </summary>
        /// <value>The name of the deleted container.</value>
        public string Name { get; set; }
    }
}
