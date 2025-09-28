using System;
using Volo.Abp.Domain.Entities.Events.Distributed;

namespace SharpAbp.Abp.MapTenancyManagement
{
    /// <summary>
    /// Event Transfer Object for MapTenant entity.
    /// Used for distributed event handling.
    /// </summary>
    public class MapTenantEto : EntityEto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the map tenant.
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Gets or sets the tenant identifier.
        /// </summary>
        public Guid TenantId { get; set; }
        
        /// <summary>
        /// Gets or sets the tenant name.
        /// </summary>
        public string TenantName { get; set; }
        
        /// <summary>
        /// Gets or sets the tenant code.
        /// </summary>
        public string Code { get; set; }
        
        /// <summary>
        /// Gets or sets the mapped code.
        /// </summary>
        public string MapCode { get; set; }
    }
}