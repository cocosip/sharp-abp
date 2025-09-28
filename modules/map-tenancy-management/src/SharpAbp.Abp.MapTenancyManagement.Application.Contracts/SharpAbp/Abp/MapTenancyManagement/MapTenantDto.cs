using System;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.MapTenancyManagement
{
    /// <summary>
    /// Data transfer object for map tenant information with audit trail support.
    /// Represents a map tenant entity for client-server communication with full audit capabilities.
    /// </summary>
    public class MapTenantDto : ExtensibleAuditedEntityDto<Guid>
    {
        /// <summary>
        /// Gets or sets the tenant identifier associated with this map tenant
        /// </summary>
        /// <value>The unique identifier of the tenant; can be null for new entities</value>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// Gets or sets the display name of the tenant
        /// </summary>
        /// <value>The human-readable name of the tenant</value>
        public string TenantName { get; set; }

        /// <summary>
        /// Gets or sets the unique code associated with the tenant
        /// </summary>
        /// <value>The tenant's unique identification code</value>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the map code for external system integration
        /// </summary>
        /// <value>The external mapping code used for tenant identification in external systems</value>
        public string MapCode { get; set; }
    }
}
