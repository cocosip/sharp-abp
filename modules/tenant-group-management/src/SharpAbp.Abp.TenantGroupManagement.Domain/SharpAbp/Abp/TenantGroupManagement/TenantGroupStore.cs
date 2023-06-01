using JetBrains.Annotations;
using SharpAbp.Abp.TenancyGrouping;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.TenantGroupManagement
{
    public class TenantGroupStore : ITenantGroupStore, ITransientDependency
    {
        protected ICurrentTenant CurrentTenant { get; }
        protected IObjectMapper ObjectMapper { get; }
        protected ITenantGroupRepository TenantGroupRepository { get; }
        protected IDistributedCache<TenantGroupCacheItem> Cache { get; }
        protected IDistributedCache<TenantGroupTenantCacheItem> TenantCache { get; }

        public TenantGroupStore(
            ICurrentTenant currentTenant,
            IObjectMapper objectMapper,
            ITenantGroupRepository tenantGroupRepository,
            IDistributedCache<TenantGroupCacheItem> cache,
            IDistributedCache<TenantGroupTenantCacheItem> tenantCache)
        {
            CurrentTenant = currentTenant;
            ObjectMapper = objectMapper;
            TenantGroupRepository = tenantGroupRepository;
            Cache = cache;
            TenantCache = tenantCache;
        }


        public virtual async Task<TenantGroupConfiguration> FindAsync(string name)
        {
            return (await GetCacheItemAsync(null, name)).Value;
        }

        public virtual async Task<TenantGroupConfiguration> FindAsync(Guid id)
        {
            return (await GetCacheItemAsync(id, null)).Value;
        }

        public virtual async Task<TenantGroupConfiguration> FindByTenantIdAsync(Guid tenantId)
        {
            return (await GetTenantCacheItemAsync(tenantId)).Value;
        }

        public TenantGroupConfiguration Find(string name)
        {
            return AsyncHelper.RunSync(() =>
            {
                return GetCacheItemAsync(null, name);
            }).Value;
        }

        public TenantGroupConfiguration Find(Guid id)
        {
            return AsyncHelper.RunSync(() =>
            {
                return GetCacheItemAsync(id, null);
            }).Value;
        }

        public TenantGroupConfiguration FindByTenantId(Guid tenantId)
        {
            return AsyncHelper.RunSync(() =>
            {
                return GetTenantCacheItemAsync(tenantId);
            }).Value;
        }

        protected virtual async Task<TenantGroupCacheItem> GetTenantCacheItemAsync(Guid tenantId)
        {
            using (CurrentTenant.Change(null))
            {
                var cacheKey = CalculateTenantCacheKey(tenantId);
                var cacheItem = await TenantCache.GetAsync(cacheKey, considerUow: true);
                if (cacheItem != null)
                {
                    return await GetCacheItemAsync(cacheItem.TenantGroupId, null);
                }

                var tenantGroup = await TenantGroupRepository.FindByTenantIdAsync(tenantId);
                if (tenantGroup != null)
                {
                    await SetTenantCacheItemAsync(cacheKey, new TenantGroupTenantCacheItem(tenantId, tenantGroup?.Id));
                    return await GetCacheItemAsync(tenantGroup.Id, null);
                }
                //租户分组为null
                return new TenantGroupCacheItem(null);
            }
        }

        protected virtual async Task SetTenantCacheItemAsync(
            string cacheKey,
            [CanBeNull] TenantGroupTenantCacheItem cacheItem)
        {
            await TenantCache.SetAsync(cacheKey, cacheItem, considerUow: true);
        }


        protected virtual string CalculateTenantCacheKey(Guid tenantId)
        {
            return TenantGroupTenantCacheItem.CalculateCacheKey(tenantId, null);
        }


        protected virtual async Task<TenantGroupCacheItem> GetCacheItemAsync(Guid? id, string name)
        {
            var cacheKey = CalculateCacheKey(id, name);

            var cacheItem = await Cache.GetAsync(cacheKey, considerUow: true);
            if (cacheItem != null)
            {
                return cacheItem;
            }

            if (id.HasValue)
            {
                using (CurrentTenant.Change(null))
                {
                    var tenantGroup = await TenantGroupRepository.FindAsync(id.Value);
                    return await SetCacheItemAsync(cacheKey, tenantGroup);
                }
            }

            if (!name.IsNullOrWhiteSpace())
            {
                using (CurrentTenant.Change(null)) //TODO: No need this if we can implement to define host side (or tenant-independent) entities!
                {
                    var tenantGroup = await TenantGroupRepository.FindByNameAsync(name);
                    return await SetCacheItemAsync(cacheKey, tenantGroup);
                }
            }

            throw new AbpException("Both id and name can't be invalid.");
        }

        protected virtual async Task<TenantGroupCacheItem> SetCacheItemAsync(string cacheKey, [CanBeNull] TenantGroup tenantGroup)
        {
            var tenantConfiguration = tenantGroup != null ? ObjectMapper.Map<TenantGroup, TenantGroupConfiguration>(tenantGroup) : null;
            var cacheItem = new TenantGroupCacheItem(tenantConfiguration);
            await Cache.SetAsync(cacheKey, cacheItem, considerUow: true);
            return cacheItem;
        }


        protected virtual string CalculateCacheKey(Guid? id, string name)
        {
            return TenantGroupCacheItem.CalculateCacheKey(id, name);
        }
    }
}
