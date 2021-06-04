using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SharpAbp.Abp.MapTenancy;
using System;
using Volo.Abp.Caching;
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
                            AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(600)
                        };
                    }
                    return null;
                });
            });

            context.Services.Replace(ServiceDescriptor.Transient<IMapTenancyConfigurationProvider, DatabaseMapTenancyConfigurationProvider>());
        }
    }
}
