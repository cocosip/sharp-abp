﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SharpAbp.Abp.TenancyGrouping;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;
using Volo.Abp.TenantManagement;

namespace SharpAbp.Abp.TenantGroupManagement
{
    /// <summary>
    /// Provides implementation for storing and retrieving tenant group configurations.
    /// </summary>
    public class TenantGroupStore : ITenantGroupStore, ITransientDependency
    {
        /// <summary>
        /// Gets the tenant group repository.
        /// </summary>
        protected ITenantGroupRepository TenantGroupRepository { get; }

        /// <summary>
        /// Gets the object mapper for mapping between tenant groups and configurations.
        /// </summary>
        protected IObjectMapper<AbpTenantManagementDomainModule> ObjectMapper { get; }

        /// <summary>
        /// Gets the current tenant accessor.
        /// </summary>
        protected ICurrentTenant CurrentTenant { get; }

        /// <summary>
        /// Gets the distributed cache for tenant group configurations.
        /// </summary>
        protected IDistributedCache<TenantGroupConfigurationCacheItem> Cache { get; }

        /// <summary>
        /// Gets the distributed cache for tenant group tenant mappings.
        /// </summary>
        protected IDistributedCache<TenantGroupTenantCacheItem> GroupCache { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantGroupStore"/> class.
        /// </summary>
        /// <param name="tenantGroupRepository">The tenant group repository.</param>
        /// <param name="objectMapper">The object mapper.</param>
        /// <param name="currentTenant">The current tenant accessor.</param>
        /// <param name="cache">The distributed cache for tenant group configurations.</param>
        /// <param name="groupCache">The distributed cache for tenant group tenant mappings.</param>
        public TenantGroupStore(
            ITenantGroupRepository tenantGroupRepository,
            IObjectMapper<AbpTenantManagementDomainModule> objectMapper,
            ICurrentTenant currentTenant,
            IDistributedCache<TenantGroupConfigurationCacheItem> cache,
            IDistributedCache<TenantGroupTenantCacheItem> groupCache)
        {
            TenantGroupRepository = tenantGroupRepository;
            ObjectMapper = objectMapper;
            CurrentTenant = currentTenant;
            Cache = cache;
            GroupCache = groupCache;
        }


        /// <summary>
        /// Finds a tenant group by its normalized name.
        /// </summary>
        /// <param name="normalizedName">The normalized name of the tenant group to find.</param>
        /// <returns>The tenant group configuration if found; otherwise, null.</returns>
        public virtual async Task<TenantGroupConfiguration> FindAsync(string normalizedName)
        {
            return (await GetCacheItemAsync(null, normalizedName)).Value;
        }

        /// <summary>
        /// Finds a tenant group by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the tenant group to find.</param>
        /// <returns>The tenant group configuration if found; otherwise, null.</returns>
        public virtual async Task<TenantGroupConfiguration> FindAsync(Guid id)
        {
            return (await GetCacheItemAsync(id, null)).Value;
        }

        /// <summary>
        /// Finds a tenant group by tenant identifier.
        /// </summary>
        /// <param name="tenantId">The tenant identifier associated with the tenant group.</param>
        /// <returns>The tenant group configuration if found; otherwise, null.</returns>
        public virtual async Task<TenantGroupConfiguration> FindByTenantIdAsync(Guid tenantId)
        {
            var cacheItem = await GetGroupCacheItemAsync(tenantId);
            return cacheItem == null ? null : await FindAsync(cacheItem.TenantGroupId);
        }

        /// <summary>
        /// Gets a list of all tenant group configurations.
        /// </summary>
        /// <param name="includeDetails">Whether to include detailed information in the results.</param>
        /// <returns>A list of tenant group configurations.</returns>
        public virtual async Task<IReadOnlyList<TenantGroupConfiguration>> GetListAsync(bool includeDetails = false)
        {
            return ObjectMapper.Map<List<TenantGroup>, List<TenantGroupConfiguration>>(
                await TenantGroupRepository.GetListAsync(includeDetails));
        }

        /// <summary>
        /// Gets a cache item for a tenant group by its identifier or normalized name.
        /// </summary>
        /// <param name="id">The unique identifier of the tenant group.</param>
        /// <param name="normalizedName">The normalized name of the tenant group.</param>
        /// <returns>The cached tenant group configuration item.</returns>
        protected virtual async Task<TenantGroupConfigurationCacheItem> GetCacheItemAsync(Guid? id, string normalizedName)
        {
            var cacheKey = CalculateCacheKey(id, normalizedName);
            var cacheItem = await Cache.GetAsync(cacheKey, considerUow: true);
            if (cacheItem?.Value != null)
            {
                return cacheItem;
            }

            if (id.HasValue)
            {
                using (CurrentTenant.Change(null)) //TODO: No need this if we can implement to define host side (or tenant-independent) entities!
                {
                    var tenantGroup = await TenantGroupRepository.FindAsync(id.Value);
                    return await SetCacheAsync(cacheKey, tenantGroup);
                }
            }

            if (!normalizedName.IsNullOrWhiteSpace())
            {
                using (CurrentTenant.Change(null)) //TODO: No need this if we can implement to define host side (or tenant-independent) entities!
                {
                    var tenant = await TenantGroupRepository.FindByNameAsync(normalizedName);
                    return await SetCacheAsync(cacheKey, tenant);
                }
            }

            throw new AbpException("Both id and normalizedName can't be invalid.");
        }


        /// <summary>
        /// Sets a tenant group configuration in the cache.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="tenantGroup">The tenant group to cache.</param>
        /// <returns>The cached tenant group configuration item.</returns>
        protected virtual async Task<TenantGroupConfigurationCacheItem> SetCacheAsync(string cacheKey, [CanBeNull] TenantGroup tenantGroup)
        {
            var tenantConfiguration = tenantGroup != null ? ObjectMapper.Map<TenantGroup, TenantGroupConfiguration>(tenantGroup) : null;
            var cacheItem = new TenantGroupConfigurationCacheItem(tenantConfiguration);
            await Cache.SetAsync(cacheKey, cacheItem, considerUow: true);
            return cacheItem;
        }

        /// <summary>
        /// Gets a cache item for a tenant group by tenant identifier.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns>The cached tenant group tenant item.</returns>
        protected virtual async Task<TenantGroupTenantCacheItem> GetGroupCacheItemAsync(Guid tenantId)
        {
            var cacheKey = CalculateGroupCacheKey(tenantId);
            using (CurrentTenant.Change(null))
            {
                var cacheItem = await GroupCache.GetOrAddAsync(
                     cacheKey,
                     async () =>
                     {
                         var tenantGroup = await TenantGroupRepository.FindByTenantIdAsync(tenantId, true);
                         if (tenantGroup != null)
                         {
                             return new TenantGroupTenantCacheItem(tenantId, tenantGroup.Id);
                         }
                         return null;
                     });
                return cacheItem;
            }
        }

        /// <summary>
        /// Calculates the cache key for a tenant group.
        /// </summary>
        /// <param name="id">The unique identifier of the tenant group.</param>
        /// <param name="normalizedName">The normalized name of the tenant group.</param>
        /// <returns>The cache key.</returns>
        protected virtual string CalculateCacheKey(Guid? id, string normalizedName)
        {
            return TenantConfigurationCacheItem.CalculateCacheKey(id, normalizedName);
        }

        /// <summary>
        /// Calculates the cache key for a tenant group tenant mapping.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns>The cache key.</returns>
        protected virtual string CalculateGroupCacheKey(Guid tenantId)
        {
            return TenantGroupTenantCacheItem.CalculateCacheKey(tenantId);
        }
    }
}