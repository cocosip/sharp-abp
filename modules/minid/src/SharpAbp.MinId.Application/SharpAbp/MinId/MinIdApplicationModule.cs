using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application;
using Volo.Abp.Caching;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.MinId
{
    [DependsOn(
        typeof(MinIdDomainModule),
        typeof(MinIdApplicationContractsModule),
        typeof(AbpDddApplicationModule),
        typeof(AbpMapperlyModule)
        )]
    public class MinIdApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddMapperlyObjectMapper<MinIdApplicationModule>();

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
            return Task.CompletedTask;
        }

    }
}
