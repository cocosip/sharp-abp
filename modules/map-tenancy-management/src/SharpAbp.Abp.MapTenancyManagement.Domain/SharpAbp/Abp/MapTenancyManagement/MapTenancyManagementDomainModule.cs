﻿using Microsoft.Extensions.Caching.Distributed;
using SharpAbp.Abp.MapTenancy;
using System;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement;

namespace SharpAbp.Abp.MapTenancyManagement
{
    [DependsOn(
        typeof(AbpCachingModule),
        typeof(AbpMapTenancyModule),
        typeof(AbpTenantManagementDomainModule),
        typeof(MapTenancyManagementDomainSharedModule)
        )]
    public class MapTenancyManagementDomainModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpDistributedCacheOptions>(options =>
            {
                options.CacheConfigurators.Add(cacheName =>
                {
                    if (cacheName == CacheNameAttribute.GetCacheName(typeof(MapTenantCacheItem)))
                    {
                        return new DistributedCacheEntryOptions()
                        {
                            AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(3600)
                        };
                    }

                    if (cacheName == CacheNameAttribute.GetCacheName(typeof(MapTenantMapCodeCacheItem)))
                    {
                        return new DistributedCacheEntryOptions()
                        {
                            AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(3600)
                        };
                    }

                    if (cacheName == CacheNameAttribute.GetCacheName(typeof(CodeCacheItem)))
                    {
                        return new DistributedCacheEntryOptions()
                        {
                            AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(3600)
                        };
                    }

                    if (cacheName == CacheNameAttribute.GetCacheName(typeof(AllMapTenantCacheItem)))
                    {
                        return new DistributedCacheEntryOptions()
                        {
                            AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(3600)
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
        }
    }
}
