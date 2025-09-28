using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;

namespace SharpAbp.Abp.AuditLogging
{
    /// <summary>
    /// Data transfer object representing a property change within an entity change
    /// </summary>
    [DisableAuditing]
    public class EntityPropertyChangeDto : EntityDto<Guid>
    {
        /// <summary>
        /// Gets or sets the tenant identifier for multi-tenancy support
        /// </summary>
        public Guid? TenantId { get; set; }
        
        /// <summary>
        /// Gets or sets the identifier of the entity change that contains this property change
        /// </summary>
        public Guid EntityChangeId { get; set; }
        
        /// <summary>
        /// Gets or sets the new value of the property after the change
        /// </summary>
        public string NewValue { get; set; }
        
        /// <summary>
        /// Gets or sets the original value of the property before the change
        /// </summary>
        public string OriginalValue { get; set; }
        
        /// <summary>
        /// Gets or sets the name of the property that was changed
        /// </summary>
        public string PropertyName { get; set; }
        
        /// <summary>
        /// Gets or sets the full type name of the property that was changed
        /// </summary>
        public string PropertyTypeFullName { get; set; }
    }
}
