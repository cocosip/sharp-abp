﻿﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.MapTenancyManagement
{
    /// <summary>
    /// Implementation of map tenant store with distributed caching and in-memory caching capabilities.
    /// Provides high-performance access to map tenancy information with automatic cache management.
    /// </summary>
    public class MapTenantStore : IMapTenantStore, ITransientDependency
    {
        protected AbpDistributedCacheOptions CacheOptions { get; }
        protected MapTenancyStoreOptions StoreOptions { get; }
        protected IGuidGenerator GuidGenerator { get; }
        protected ICurrentTenant CurrentTenant { get; }
        protected IMapTenantRepository MapTenantRepository { get; }
        protected IMapTenancyStoreInMemoryCache StoreCache { get; }
        protected IDistributedCache DistributedCache { get; }
        protected IAbpDistributedLock DistributedLock { get; }

        /// <summary>
        /// Initializes a new instance of the MapTenantStore class
        /// </summary>
        /// <param name="cacheOptions">Distributed cache configuration options</param>
        /// <param name="storeOptions">Map tenancy store configuration options</param>
        /// <param name="guidGenerator">GUID generator for creating unique identifiers</param>
        /// <param name="currentTenant">Current tenant context provider</param>
        /// <param name="mapTenantRepository">Repository for map tenant data access</param>
        /// <param name="storeCache">In-memory cache for storing map tenancy data</param>
        /// <param name="distributedCache">Distributed cache for cross-instance synchronization</param>
        /// <param name="distributedLock">Distributed locking mechanism for cache coordination</param>
        public MapTenantStore(
            IOptions<AbpDistributedCacheOptions> cacheOptions,
            IOptions<MapTenancyStoreOptions> storeOptions,
            IGuidGenerator guidGenerator,
            ICurrentTenant currentTenant,
            IMapTenantRepository mapTenantRepository,
            IMapTenancyStoreInMemoryCache storeCache,
            IDistributedCache distributedCache,
            IAbpDistributedLock distributedLock)
        {
            CacheOptions = cacheOptions.Value;
            StoreOptions = storeOptions.Value;

            GuidGenerator = guidGenerator;
            CurrentTenant = currentTenant;
            MapTenantRepository = mapTenantRepository;
            StoreCache = storeCache;
            DistributedCache = distributedCache;
            DistributedLock = distributedLock;
        }

        /// <summary>
        /// Retrieves a map tenancy tenant by its tenant identifier with caching support
        /// </summary>
        /// <param name="tenantId">The unique identifier of the tenant</param>
        /// <returns>The map tenancy tenant if found; otherwise null</returns>
        public virtual async Task<MapTenancyTenant> GetByTenantIdAsync(Guid tenantId)
        {
            using (await StoreCache.SyncSemaphore.LockAsync())
            {
                await EnsureCacheIsUptoDateAsync();
                return StoreCache.GetByTenantIdOrNull(tenantId);
            }
        }

        /// <summary>
        /// Retrieves a map tenancy tenant by its code with caching support
        /// </summary>
        /// <param name="code">The unique code of the tenant</param>
        /// <returns>The map tenancy tenant if found; otherwise null</returns>
        public virtual async Task<MapTenancyTenant> GetByCodeAsync(string code)
        {
            using (await StoreCache.SyncSemaphore.LockAsync())
            {
                await EnsureCacheIsUptoDateAsync();
                return StoreCache.GetByCodeOrNull(code);
            }
        }

        /// <summary>
        /// Retrieves a map tenancy tenant by its map code with caching support
        /// </summary>
        /// <param name="mapCode">The unique map code of the tenant</param>
        /// <returns>The map tenancy tenant if found; otherwise null</returns>
        public virtual async Task<MapTenancyTenant> GetByMapCodeAsync(string mapCode)
        {
            using (await StoreCache.SyncSemaphore.LockAsync())
            {
                await EnsureCacheIsUptoDateAsync();
                return StoreCache.GetByMapCodeOrNull(mapCode);
            }
        }

        /// <summary>
        /// Retrieves all map tenancy tenants from the store with caching support
        /// </summary>
        /// <returns>A read-only list of all map tenancy tenants</returns>
        public virtual async Task<IReadOnlyList<MapTenancyTenant>> GetAllAsync()
        {
            using (await StoreCache.SyncSemaphore.LockAsync())
            {
                await EnsureCacheIsUptoDateAsync();
                return StoreCache.GetAll();
            }
        }

        /// <summary>
        /// Resets the distributed cache and optionally resets the last check time
        /// </summary>
        /// <param name="resetLastCheckTime">Whether to reset the last check time; defaults to false</param>
        /// <returns>A task representing the asynchronous reset operation</returns>
        public virtual async Task ResetAsync(bool resetLastCheckTime = false)
        {
            using (await StoreCache.SyncSemaphore.LockAsync())
            {
                var cacheKey = GetCommonStampCacheKey();
                if (resetLastCheckTime)
                {
                    StoreCache.LastCheckTime = null;
                }

                await DistributedCache.RemoveAsync(cacheKey);
            }
        }


        /// <summary>
        /// Ensures that the in-memory cache is up-to-date by checking against the distributed cache
        /// </summary>
        /// <returns>A task representing the asynchronous cache validation operation</returns>
        protected virtual async Task EnsureCacheIsUptoDateAsync()
        {
            if (StoreCache.LastCheckTime.HasValue &&
                DateTime.Now.Subtract(StoreCache.LastCheckTime.Value).TotalSeconds < 30)
            {
                return;
            }

            var stampInDistributedCache = await GetOrSetStampInDistributedCache();

            if (stampInDistributedCache == StoreCache.CacheStamp)
            {
                StoreCache.LastCheckTime = DateTime.Now;
                return;
            }

            await UpdateInMemoryStoreCache();

            StoreCache.CacheStamp = stampInDistributedCache;
            StoreCache.LastCheckTime = DateTime.Now;
        }

        /// <summary>
        /// Updates the in-memory store cache by loading fresh data from the repository
        /// </summary>
        /// <returns>A task representing the asynchronous cache update operation</returns>
        protected virtual async Task UpdateInMemoryStoreCache()
        {
            using (CurrentTenant.Change(null))
            {
                var mapTenants = await MapTenantRepository.GetListAsync();
                await StoreCache.FillAsync(mapTenants);
            }
        }

        /// <summary>
        /// Gets or sets the cache stamp in the distributed cache for synchronization purposes
        /// </summary>
        /// <returns>The current cache stamp identifier</returns>
        /// <exception cref="Volo.Abp.AbpException">Thrown when distributed lock cannot be acquired</exception>
        protected virtual async Task<string> GetOrSetStampInDistributedCache()
        {
            var cacheKey = GetCommonStampCacheKey();

            var stampInDistributedCache = await DistributedCache.GetStringAsync(cacheKey);
            if (stampInDistributedCache != null)
            {
                return stampInDistributedCache;
            }

            await using (var commonLockHandle = await DistributedLock.TryAcquireAsync(GetCommonDistributedLockKey(), TimeSpan.FromMinutes(2)))
            {
                if (commonLockHandle == null)
                {
                    /* This request will fail */
                    throw new AbpException(
                        "Could not acquire distributed lock for map tenancy common stamp check!"
                    );
                }

                stampInDistributedCache = await DistributedCache.GetStringAsync(cacheKey);
                if (stampInDistributedCache != null)
                {
                    return stampInDistributedCache;
                }

                stampInDistributedCache = Guid.NewGuid().ToString();

                await DistributedCache.SetStringAsync(
                    cacheKey,
                    stampInDistributedCache,
                    new DistributedCacheEntryOptions
                    {
                        SlidingExpiration = StoreOptions.StampExpiration
                    }
                );
            }

            return stampInDistributedCache;
        }


        /// <summary>
        /// Gets the cache key for the common stamp used in distributed cache synchronization
        /// </summary>
        /// <returns>The formatted cache key for the common stamp</returns>
        protected virtual string GetCommonStampCacheKey()
        {
            return $"{CacheOptions.KeyPrefix}_AbpInMemoryMapTenancyCacheStamp";
        }


        /// <summary>
        /// Gets the distributed lock key for coordinating cache updates across multiple instances
        /// </summary>
        /// <returns>The formatted distributed lock key</returns>
        protected virtual string GetCommonDistributedLockKey()
        {
            return $"{CacheOptions.KeyPrefix}_Common_AbpMapTenancyUpdateLock";
        }
    }
}
