using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;

namespace SharpAbp.Abp.AuditLogging
{
    /// <summary>
    /// Data transfer object for paged entity change requests with filtering options
    /// </summary>
    public class EntityChangePagedRequestDto : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// Gets or sets the audit log identifier filter for entity changes
        /// </summary>
        public Guid? AuditLogId { get; set; }
        /// <summary>
        /// Gets or sets the start time for filtering entity changes
        /// </summary>
        public DateTime? StartTime { get; set; }
        
        /// <summary>
        /// Gets or sets the end time for filtering entity changes
        /// </summary>
        public DateTime? EndTime { get; set; }
        
        /// <summary>
        /// Gets or sets the type of entity change to filter by (Created, Updated, Deleted)
        /// </summary>
        public EntityChangeType? ChangeType { get; set; }
        
        /// <summary>
        /// Gets or sets the entity identifier filter for entity changes
        /// </summary>
        public string EntityId { get; set; }
        
        /// <summary>
        /// Gets or sets the full type name filter for entity changes
        /// </summary>
        public string EntityTypeFullName { get; set; }
    }
}
