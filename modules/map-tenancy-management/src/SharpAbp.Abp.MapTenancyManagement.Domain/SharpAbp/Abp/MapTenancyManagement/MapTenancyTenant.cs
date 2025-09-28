﻿using System;

namespace SharpAbp.Abp.MapTenancyManagement
{
    /// <summary>
    /// Represents a map tenancy tenant entity that provides mapping between tenant identifiers and codes.
    /// This class encapsulates tenant information for multi-tenancy scenarios with custom code mapping.
    /// </summary>
    public class MapTenancyTenant
    {
        /// <summary>
        /// Gets or sets the unique identifier of the tenant
        /// </summary>
        /// <value>The tenant's unique identifier</value>
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
        /// Initializes a new instance of the MapTenancyTenant class with default values
        /// </summary>
        public MapTenancyTenant()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the MapTenancyTenant class with the specified parameters
        /// </summary>
        /// <param name="tenantId">The unique identifier of the tenant</param>
        /// <param name="tenantName">The display name of the tenant</param>
        /// <param name="code">The unique code associated with the tenant</param>
        /// <param name="mapCode">The map code for external system integration</param>
        public MapTenancyTenant(Guid tenantId, string tenantName, string code, string mapCode)
        {
            TenantId = tenantId;
            TenantName = tenantName;
            Code = code;
            MapCode = mapCode;
        }
    }
}
