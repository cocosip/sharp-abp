using System;
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

        public virtual async Task<MapTenancyTenant> GetByTenantIdAsync(Guid tenantId)
        {
            using (await StoreCache.SyncSemaphore.LockAsync())
            {
                await EnsureCacheIsUptoDateAsync();
                return StoreCache.GetByTenantIdOrNull(tenantId);
            }
        }

        public virtual async Task<MapTenancyTenant> GetByCodeAsync(string code)
        {
            using (await StoreCache.SyncSemaphore.LockAsync())
            {
                await EnsureCacheIsUptoDateAsync();
                return StoreCache.GetByCodeOrNull(code);
            }
        }

        public virtual async Task<MapTenancyTenant> GetByMapCodeAsync(string mapCode)
        {
            using (await StoreCache.SyncSemaphore.LockAsync())
            {
                await EnsureCacheIsUptoDateAsync();
                return StoreCache.GetByMapCodeOrNull(mapCode);
            }
        }

        public virtual async Task<IReadOnlyList<MapTenancyTenant>> GetAllAsync()
        {
            using (await StoreCache.SyncSemaphore.LockAsync())
            {
                await EnsureCacheIsUptoDateAsync();
                return StoreCache.GetAll();
            }
        }

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

        protected virtual async Task UpdateInMemoryStoreCache()
        {
            using (CurrentTenant.Change(null))
            {
                var mapTenants = await MapTenantRepository.GetListAsync();
                await StoreCache.FillAsync(mapTenants);
            }
        }

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


        protected virtual string GetCommonStampCacheKey()
        {
            return $"{CacheOptions.KeyPrefix}_AbpInMemoryMapTenancyCacheStamp";
        }


        protected virtual string GetCommonDistributedLockKey()
        {
            return $"{CacheOptions.KeyPrefix}_Common_AbpMapTenancyUpdateLock";
        }
    }
}
