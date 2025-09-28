using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace SharpAbp.Abp.MapTenancyManagement
{
    /// <summary>
    /// Represents a mapping between a tenant and its code mappings.
    /// </summary>
    public class MapTenant : AuditedAggregateRoot<Guid>
    {
        /// <summary>
        /// Gets or sets the tenant identifier.
        /// </summary>
        public virtual Guid TenantId { get; set; }
        
        /// <summary>
        /// Gets or sets the tenant name.
        /// </summary>
        public virtual string TenantName { get; set; }
        
        /// <summary>
        /// Gets or sets the tenant code.
        /// </summary>
        public virtual string Code { get; set; }
        
        /// <summary>
        /// Gets or sets the mapped code.
        /// </summary>
        public virtual string MapCode { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapTenant"/> class.
        /// </summary>
        public MapTenant()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapTenant"/> class with specified parameters.
        /// </summary>
        /// <param name="id">The map tenant identifier.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="tenantName">The tenant name.</param>
        /// <param name="code">The tenant code.</param>
        /// <param name="mapCode">The mapped code.</param>
        public MapTenant(Guid id, Guid tenantId, string tenantName, string code, string mapCode) : base(id)
        {
            TenantId = tenantId;
            TenantName = tenantName;
            Code = code;
            MapCode = mapCode;
        }

        /// <summary>
        /// Updates the map tenant with new values.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="tenantName">The tenant name.</param>
        /// <param name="code">The tenant code.</param>
        /// <param name="mapCode">The mapped code.</param>
        public void Update(Guid tenantId, string tenantName, string code, string mapCode)
        {
            TenantId = tenantId;
            TenantName = tenantName;
            Code = code;
            MapCode = mapCode;
        }
    }
}