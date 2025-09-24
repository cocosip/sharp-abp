﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SharpAbp.Abp.TenancyGrouping;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;

namespace SharpAbp.Abp.TenantGroupManagement
{
    /// <summary>
    /// Manages caching operations for tenant groups.
    /// </summary>
    public class TenantGroupCacheManager : ITenantGroupCacheManager, ITransientDependency
    {
        /// <summary>
        /// Gets the current tenant.
        /// </summary>
        protected ICurrentTenant CurrentTenant { get; }
        
        /// <summary>
        /// Gets the object mapper.
        /// </summary>
        protected IObjectMapper ObjectMapper { get; }
        
        /// <summary>
        /// Gets the tenant group repository.
        /// </summary>
        protected ITenantGroupRepository TenantGroupRepository { get; }
        
        /// <summary>
        /// Gets the distributed cache for tenant group configurations.
        /// </summary>
        protected IDistributedCache<TenantGroupConfigurationCacheItem> Cache { get; }
        
        /// <summary>
        /// Gets the distributed cache for tenant group tenants.
        /// </summary>
        protected IDistributedCache<TenantGroupTenantCacheItem> GroupCache { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="TenantGroupCacheManager"/> class.
        /// </summary>
        /// <param name="currentTenant">The current tenant.</param>
        /// <param name="objectMapper">The object mapper.</param>
        /// <param name="tenantGroupRepository">The tenant group repository.</param>
        /// <param name="cache">The distributed cache for tenant group configurations.</param>
        /// <param name="groupCache">The distributed cache for tenant group tenants.</param>
        public TenantGroupCacheManager(
            ICurrentTenant currentTenant,
            IObjectMapper objectMapper,
            ITenantGroupRepository tenantGroupRepository,
            IDistributedCache<TenantGroupConfigurationCacheItem> cache,
            IDistributedCache<TenantGroupTenantCacheItem> groupCache)
        {
            CurrentTenant = currentTenant;
            ObjectMapper = objectMapper;
            TenantGroupRepository = tenantGroupRepository;
            Cache = cache;
            GroupCache = groupCache;
        }

        /// <summary>
        /// Removes tenant group cache entries by ID and/or normalized name.
        /// </summary>
        /// <param name="id">The tenant group ID.</param>
        /// <param name="normalizedName">The normalized name of the tenant group.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="AbpException">Thrown when both id and normalizedName are invalid.</exception>
        public virtual async Task RemoveAsync(Guid? id, string normalizedName, CancellationToken cancellationToken = default)
        {
            if (id == null && normalizedName.IsNullOrWhiteSpace())
            {
                throw new AbpException("Tenant group id or normalized name is required.");
            }

            using (CurrentTenant.Change(null))
            {
                var tenants = new List<Guid>();
                var cacheKeys = new List<string>();
                
                if (id.HasValue)
                {
                    cacheKeys.Add(CalculateCacheKey(id, null));
                }

                if (!normalizedName.IsNullOrWhiteSpace())
                {
                    cacheKeys.Add(CalculateCacheKey(null, normalizedName));
                }

                if (id.HasValue && !normalizedName.IsNullOrWhiteSpace())
                {
                    cacheKeys.Add(CalculateCacheKey(id, normalizedName));
                }

                foreach (var cacheKey in cacheKeys)
                {
                    var cacheItem = await Cache.GetAsync(cacheKey, token: cancellationToken);
                    if (cacheItem?.Value != null)
                    {
                        foreach (var tenantId in cacheItem.Value.Tenants)
                        {
                            tenants.AddIfNotContains(tenantId);
                        }
                    }
                }

                var groupCacheKeys = tenants.Distinct().Select(CalculateGroupCacheKey).ToList();
                await Cache.RemoveManyAsync(cacheKeys, token: cancellationToken);
                await GroupCache.RemoveManyAsync(groupCacheKeys, token: cancellationToken);
            }
        }

        /// <summary>
        /// Calculates the cache key for a tenant group.
        /// </summary>
        /// <param name="id">The tenant group ID.</param>
        /// <param name="normalizedName">The normalized name of the tenant group.</param>
        /// <returns>The calculated cache key.</returns>
        protected virtual string CalculateCacheKey(Guid? id, string normalizedName)
        {
            return TenantGroupConfigurationCacheItem.CalculateCacheKey(id, normalizedName);
        }

        /// <summary>
        /// Calculates the cache key for a tenant group tenant.
        /// </summary>
        /// <param name="tenantId">The tenant ID.</param>
        /// <returns>The calculated cache key.</returns>
        protected virtual string CalculateGroupCacheKey(Guid tenantId)
        {
            return TenantGroupTenantCacheItem.CalculateCacheKey(tenantId);
        }
    }
}