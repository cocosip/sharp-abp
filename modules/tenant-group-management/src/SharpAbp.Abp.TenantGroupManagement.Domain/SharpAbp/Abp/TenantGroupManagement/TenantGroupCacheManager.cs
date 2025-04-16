using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SharpAbp.Abp.TenancyGrouping;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;

namespace SharpAbp.Abp.TenantGroupManagement
{
    public class TenantGroupCacheManager : ITenantGroupCacheManager, ITransientDependency
    {
        protected ICurrentTenant CurrentTenant { get; }
        protected IObjectMapper ObjectMapper { get; }
        protected ITenantGroupRepository TenantGroupRepository { get; }
        protected IDistributedCache<TenantGroupConfigurationCacheItem> Cache { get; }
        protected IDistributedCache<TenantGroupTenantCacheItem> TenantCache { get; }
        public TenantGroupCacheManager(
            ICurrentTenant currentTenant,
            IObjectMapper objectMapper,
            ITenantGroupRepository tenantGroupRepository,
            IDistributedCache<TenantGroupConfigurationCacheItem> cache,
            IDistributedCache<TenantGroupTenantCacheItem> tenantCache)
        {
            CurrentTenant = currentTenant;
            ObjectMapper = objectMapper;
            TenantGroupRepository = tenantGroupRepository;
            Cache = cache;
            TenantCache = tenantCache;
        }

        public virtual async Task UpdateTenantGroupCacheAsync(Guid id, CancellationToken cancellationToken = default)
        {
            using (CurrentTenant.Change(null))
            {
                var tenantGroup = await TenantGroupRepository.FindAsync(id, true, cancellationToken);
                if (tenantGroup != null)
                {
                    var cacheKey = CalculateCacheKey(tenantGroup.Id, tenantGroup.Name);
                    await SetCacheItemAsync(cacheKey, tenantGroup);

                    var tenantGroupTenants = new List<KeyValuePair<string, TenantGroupTenantCacheItem>>();

                    foreach (var item in tenantGroup.Tenants)
                    {
                        var tenantGroupTenant = new TenantGroupTenantCacheItem(item.TenantId, item.TenantGroupId);
                        var key = CalculateTenantCacheKey(item.TenantId);
                        tenantGroupTenants.Add(new KeyValuePair<string, TenantGroupTenantCacheItem>(key, tenantGroupTenant));
                    }
                    if (tenantGroupTenants.Count != 0)
                    {
                        await TenantCache.SetManyAsync(tenantGroupTenants);
                    }
                }
            }
        }

        public virtual async Task RemoveTenantGroupCacheAsync(
            Guid id,
            string name,
            List<Guid> tenantIds,
            CancellationToken cancellationToken = default)
        {

            using (CurrentTenant.Change(null))
            {
                var cacheKey = CalculateCacheKey(id, name);
                await Cache.RemoveAsync(cacheKey, token: cancellationToken);
                var keys = tenantIds.Select(CalculateTenantCacheKey).ToList();
                await TenantCache.RemoveManyAsync(keys, token: cancellationToken);
            }
        }


        protected virtual async Task SetTenantCacheItemAsync(
            string cacheKey,
            [CanBeNull] TenantGroupTenantCacheItem cacheItem)
        {
            await TenantCache.SetAsync(cacheKey, cacheItem, considerUow: true);
        }

        protected virtual async Task<TenantGroupConfigurationCacheItem> SetCacheItemAsync(string cacheKey, [CanBeNull] TenantGroup tenantGroup)
        {
            var tenantGroupConfiguration = tenantGroup != null ? ObjectMapper.Map<TenantGroup, TenantGroupConfiguration>(tenantGroup) : null;
            var cacheItem = new TenantGroupConfigurationCacheItem(tenantGroupConfiguration);
            await Cache.SetAsync(cacheKey, cacheItem, considerUow: true);
            return cacheItem;
        }

        protected virtual string CalculateTenantCacheKey(Guid tenantId)
        {
            return TenantGroupTenantCacheItem.CalculateCacheKey(tenantId);
        }


        protected virtual string CalculateCacheKey(Guid? id, string name)
        {
            return TenantGroupConfigurationCacheItem.CalculateCacheKey(id, name);
        }
    }
}
