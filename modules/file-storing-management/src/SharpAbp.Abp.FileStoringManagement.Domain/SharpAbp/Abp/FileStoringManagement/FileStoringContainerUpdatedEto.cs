using System;
using Volo.Abp.Domain.Entities.Events.Distributed;

namespace SharpAbp.Abp.FileStoringManagement
{
    /// <summary>
    /// Represents an event transfer object that is published when a file storing container is updated.
    /// This event carries information about container changes and is used for distributed communication
    /// between services to maintain consistency across the system.
    /// </summary>
    [Serializable]
    public class FileStoringContainerUpdatedEto : EtoBase
    {
        /// <summary>
        /// Gets or sets the unique identifier of the updated file storing container.
        /// </summary>
        /// <value>The container's unique identifier.</value>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Gets or sets the tenant identifier that owns this container.
        /// Null indicates this container belongs to the host.
        /// </summary>
        /// <value>The tenant identifier, or null for host-owned containers.</value>
        public Guid? TenantId { get; set; }
        
        /// <summary>
        /// Gets or sets the previous name of the container before the update.
        /// This is used to track container name changes for cache invalidation and reference updates.
        /// </summary>
        /// <value>The container's previous name.</value>
        public string OldName { get; set; }
        
        /// <summary>
        /// Gets or sets the new name of the container after the update.
        /// This represents the current name of the container.
        /// </summary>
        /// <value>The container's new name.</value>
        public string Name { get; set; }
    }

}
