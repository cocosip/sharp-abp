using System;
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
    public class TenantGroupStore : ITenantGroupStore, ITransientDependency
    {
        protected ITenantGroupRepository TenantGroupRepository { get; }
        protected IObjectMapper<AbpTenantManagementDomainModule> ObjectMapper { get; }
        protected ICurrentTenant CurrentTenant { get; }
        protected IDistributedCache<TenantGroupConfigurationCacheItem> Cache { get; }
        protected IDistributedCache<TenantGroupTenantCacheItem> GroupCache { get; }

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


        public virtual async Task<TenantGroupConfiguration> FindAsync(string normalizedName)
        {
            return (await GetCacheItemAsync(null, normalizedName)).Value;
        }

        public virtual async Task<TenantGroupConfiguration> FindAsync(Guid id)
        {
            return (await GetCacheItemAsync(id, null)).Value;
        }

        public virtual async Task<TenantGroupConfiguration> FindByTenantIdAsync(Guid tenantId)
        {
            var cacheItem = await GetGroupCacheItemAsync(tenantId);
            return cacheItem == null ? null : await FindAsync(cacheItem.TenantGroupId);
        }

        public virtual async Task<IReadOnlyList<TenantGroupConfiguration>> GetListAsync(bool includeDetails = false)
        {
            return ObjectMapper.Map<List<TenantGroup>, List<TenantGroupConfiguration>>(
                await TenantGroupRepository.GetListAsync(includeDetails));
        }

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


        protected virtual async Task<TenantGroupConfigurationCacheItem> SetCacheAsync(string cacheKey, [CanBeNull] TenantGroup tenantGroup)
        {
            var tenantConfiguration = tenantGroup != null ? ObjectMapper.Map<TenantGroup, TenantGroupConfiguration>(tenantGroup) : null;
            var cacheItem = new TenantGroupConfigurationCacheItem(tenantConfiguration);
            await Cache.SetAsync(cacheKey, cacheItem, considerUow: true);
            return cacheItem;
        }

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

        protected virtual string CalculateCacheKey(Guid? id, string normalizedName)
        {
            return TenantConfigurationCacheItem.CalculateCacheKey(id, normalizedName);
        }

        protected virtual string CalculateGroupCacheKey(Guid tenantId)
        {
            return TenantGroupTenantCacheItem.CalculateCacheKey(tenantId);
        }
    }
}
