﻿using System;

namespace SharpAbp.Abp.TenantGroupManagement
{
    /// <summary>
    /// Represents a cache item that stores the relationship between a tenant and its associated tenant group.
    /// This class is used for caching tenant-to-tenant-group mappings to improve performance.
    /// </summary>
    public class TenantGroupTenantCacheItem
    {
        /// <summary>
        /// The format string used to generate cache keys for tenant-to-tenant-group mappings.
        /// </summary>
        private const string CacheKeyFormat = "t:{0}";
        
        /// <summary>
        /// Gets or sets the identifier of the tenant.
        /// </summary>
        public Guid TenantId { get; set; }
        
        /// <summary>
        /// Gets or sets the identifier of the tenant group that contains this tenant.
        /// </summary>
        public Guid TenantGroupId { get; set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="TenantGroupTenantCacheItem"/> class.
        /// </summary>
        /// <param name="tenantId">The identifier of the tenant.</param>
        /// <param name="tenantGroupId">The identifier of the tenant group.</param>
        public TenantGroupTenantCacheItem(Guid tenantId, Guid tenantGroupId)
        {
            TenantId = tenantId;
            TenantGroupId = tenantGroupId;
        }
        
        /// <summary>
        /// Calculates the cache key for a given tenant identifier.
        /// </summary>
        /// <param name="tenantId">The identifier of the tenant for which to generate the cache key.</param>
        /// <returns>A formatted cache key string that can be used to store or retrieve this tenant's cache item.</returns>
        public static string CalculateCacheKey(Guid tenantId)
        {
            return string.Format(CacheKeyFormat, tenantId);
        }
    }
}
