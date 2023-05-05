using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

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
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }


        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<MinIdOptions>(options => { });

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
            return Task.CompletedTask;
        }

    }
}
