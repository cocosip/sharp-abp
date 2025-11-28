using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;

namespace SharpAbp.Abp.AuditLogging
{
    /// <summary>
    /// Data transfer object representing an entity change with associated username information
    /// </summary>
    public class EntityChangeWithUsernameDto : ExtensibleEntityDto<Guid>
    {
        /// <summary>
        /// Gets or sets the identifier of the audit log that contains this entity change
        /// </summary>
        public Guid AuditLogId { get; set; }
        
        /// <summary>
        /// Gets or sets the tenant identifier for multi-tenancy support
        /// </summary>
        public Guid? TenantId { get; set; }
        
        /// <summary>
        /// Gets or sets the date and time when the entity change occurred
        /// </summary>
        public DateTime ChangeTime { get; set; }
        
        /// <summary>
        /// Gets or sets the type of change performed on the entity (Created, Updated, Deleted)
        /// </summary>
        public EntityChangeType ChangeType { get; set; }
        
        /// <summary>
        /// Gets or sets the identifier of the changed entity
        /// </summary>
        public string EntityId { get; set; }
        
        /// <summary>
        /// Gets or sets the name of the changed entity
        /// </summary>
        public string EntityName { get; set; }
        
        /// <summary>
        /// Gets or sets the full type name of the changed entity
        /// </summary>
        public string EntityTypeFullName { get; set; }
        
        /// <summary>
        /// Gets or sets the username of the user who performed the entity change
        /// </summary>
        public string UserName { get; set; }
        
        /// <summary>
        /// Gets or sets the list of property changes for this entity
        /// </summary>
        public List<EntityPropertyChangeDto> PropertyChanges { get; set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityChangeWithUsernameDto"/> class
        /// </summary>
        public EntityChangeWithUsernameDto()
        {
            PropertyChanges = new List<EntityPropertyChangeDto>();
        }
    }
}
