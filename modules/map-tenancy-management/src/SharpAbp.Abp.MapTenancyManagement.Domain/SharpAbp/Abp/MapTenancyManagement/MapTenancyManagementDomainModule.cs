using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.MapTenancy;
using System;
using System.Threading.Tasks;
using Volo.Abp.AutoMapper;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.MapTenancyManagement
{
    [DependsOn(
        typeof(AbpCachingModule),
        typeof(AbpMapTenancyModule),
        typeof(AbpAutoMapperModule),
        typeof(AbpTenantManagementDomainModule),
        typeof(MapTenancyManagementDomainSharedModule)
        )]
    public class MapTenancyManagementDomainModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<MapTenancyManagementDomainModule>();
            });

            context.Services.AddAutoMapperObjectMapper<MapTenancyManagementDomainModule>();

            Configure<AbpDistributedCacheOptions>(options =>
            {
                options.CacheConfigurators.Add(cacheName =>
                {
                    if (cacheName == CacheNameAttribute.GetCacheName(typeof(MapTenantCacheItem)))
                    {
                        return new DistributedCacheEntryOptions()
                        {
                            SlidingExpiration = TimeSpan.FromSeconds(1800)
                        };
                    }
                    return null;
                });

                options.CacheConfigurators.Add(cacheName =>
                {
                    if (cacheName == CacheNameAttribute.GetCacheName(typeof(MapTenantMapCodeCacheItem)))
                    {
                        return new DistributedCacheEntryOptions()
                        {
                            SlidingExpiration = TimeSpan.FromSeconds(1800)
                        };
                    }
                    return null;
                });

                options.CacheConfigurators.Add(cacheName =>
                {
                    if (cacheName == CacheNameAttribute.GetCacheName(typeof(CodeCacheItem)))
                    {
                        return new DistributedCacheEntryOptions()
                        {
                            SlidingExpiration = TimeSpan.FromSeconds(1800)
                        };
                    }
                    return null;
                });

                options.CacheConfigurators.Add(cacheName =>
                {
                    if (cacheName == CacheNameAttribute.GetCacheName(typeof(AllMapTenantCacheItem)))
                    {
                        return new DistributedCacheEntryOptions()
                        {
                            SlidingExpiration = TimeSpan.FromSeconds(1800)
                        };
                    }
                    return null;
                });
            });

            Configure<AbpDistributedEntityEventOptions>(options =>
            {
                options.AutoEventSelectors.Add<MapTenant>();
                options.EtoMappings.Add<MapTenant, MapTenantEto>();
            });
            return Task.CompletedTask;
        }

    }
}
