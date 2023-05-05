using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.TenancyGrouping;
using System;
using System.Threading.Tasks;
using Volo.Abp.AutoMapper;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.TenantGroupManagement
{
    [DependsOn(
        typeof(AbpCachingModule),
        typeof(AbpTenancyGroupingModule),
        typeof(AbpTenantManagementDomainModule),
        typeof(TenantGroupManagementDomainSharedModule),
        typeof(AbpAutoMapperModule)
        )]
    public class TenantGroupManagementDomainModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<TenantGroupManagementDomainModule>(validate: true);
            });

            context.Services.AddAutoMapperObjectMapper<TenantGroupManagementDomainModule>();

            Configure<AbpDistributedCacheOptions>(options =>
            {
                options.CacheConfigurators.Add(cacheName =>
                {
                    if (cacheName == CacheNameAttribute.GetCacheName(typeof(TenantGroupCacheItem)))
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
                    if (cacheName == CacheNameAttribute.GetCacheName(typeof(TenantGroupTenantCacheItem)))
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
                options.AutoEventSelectors.Add<TenantGroup>();
                options.EtoMappings.Add<TenantGroup, TenantGroupEto>();
            });

            return Task.CompletedTask;
        }


    }

}
