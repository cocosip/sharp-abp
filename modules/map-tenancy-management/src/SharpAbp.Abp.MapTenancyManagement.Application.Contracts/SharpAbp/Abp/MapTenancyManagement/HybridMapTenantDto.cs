using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace SharpAbp.Abp.MapTenancyManagement
{
    /// <summary>
    /// Data transfer object for hybrid map tenant information with concurrency control.
    /// Combines tenant management and map tenant functionality in a unified representation.
    /// </summary>
    public class HybridMapTenantDto : ExtensibleEntityDto<Guid>, IHasConcurrencyStamp
    {
        /// <summary>
        /// Gets or sets the tenant identifier
        /// </summary>
        /// <value>The unique identifier of the tenant</value>
        public Guid TenantId { get; set; }

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

        /// <summary>
        /// Gets or sets the concurrency stamp for optimistic concurrency control
        /// </summary>
        /// <value>The concurrency control token to prevent concurrent update conflicts</value>
        public string ConcurrencyStamp { get; set; }
    }
}