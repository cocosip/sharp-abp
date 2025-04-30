using System;
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
    public class TenantGroupCacheManager : ITenantGroupCacheManager, ITransientDependency
    {
        protected ICurrentTenant CurrentTenant { get; }
        protected IObjectMapper ObjectMapper { get; }
        protected ITenantGroupRepository TenantGroupRepository { get; }
        protected IDistributedCache<TenantGroupConfigurationCacheItem> Cache { get; }
        protected IDistributedCache<TenantGroupTenantCacheItem> GroupCache { get; }
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


        public virtual async Task RemoveAsync(Guid? id, string normalizedName, CancellationToken cancellationToken = default)
        {
            if (id == null && normalizedName.IsNullOrWhiteSpace())
            {
                throw new AbpException("Both id and normalizedName can't be invalid.");
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
                    var cacheItem = await Cache.GetAsync(cacheKey);
                    if (cacheItem?.Value != null)
                    {
                        foreach (var tenantId in cacheItem.Value.Tenants)
                        {
                            tenants.AddIfNotContains(tenantId);
                        }
                    }
                }

                var groupCacheKeys = tenants.Distinct().Select(CalculateGroupCacheKey).ToList();
                await Cache.RefreshManyAsync(cacheKeys, token: cancellationToken);
                await GroupCache.RemoveManyAsync(groupCacheKeys, token: cancellationToken);
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
