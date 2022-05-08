using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Caching;
using Volo.Abp.Modularity;

namespace SharpAbp.MinId
{
    [DependsOn(
        typeof(MinIdDomainModule),
        typeof(MinIdApplicationContractsModule),
        typeof(AbpDddApplicationModule),
        typeof(AbpAutoMapperModule)
        )]
    public class MinIdApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAutoMapperObjectMapper<MinIdApplicationModule>();
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<MinIdApplicationModule>(validate: false);
            });

            Configure<AbpDistributedCacheOptions>(options =>
            {
                options.CacheConfigurators.Add(cacheName =>
                {
                    if (cacheName == CacheNameAttribute.GetCacheName(typeof(MinIdTokenCacheItem)))
                    {
                        return new DistributedCacheEntryOptions()
                        {
                            AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(600)
                        };
                    }
                    return null;
                });
            });

        }
    }
}
