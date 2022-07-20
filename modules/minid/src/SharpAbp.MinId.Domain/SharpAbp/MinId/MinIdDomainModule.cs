using Microsoft.Extensions.Caching.Distributed;
using System;
using Volo.Abp.Caching;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace SharpAbp.MinId
{
    [DependsOn(
        typeof(AbpDddDomainModule),
        typeof(MinIdDomainSharedModule),
        typeof(AbpCachingModule)
        )]
    public class MinIdDomainModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<MinIdOptions>(options =>
            {

            });

            Configure<AbpDistributedCacheOptions>(options =>
            {
                options.CacheConfigurators.Add(cacheName =>
                {
                    if (cacheName == CacheNameAttribute.GetCacheName(typeof(MinIdInfoCacheItem)))
                    {
                        return new DistributedCacheEntryOptions()
                        {
                            SlidingExpiration = TimeSpan.FromSeconds(1800)
                        };
                    }
                    return null;
                });
            });
        }
    }
}
