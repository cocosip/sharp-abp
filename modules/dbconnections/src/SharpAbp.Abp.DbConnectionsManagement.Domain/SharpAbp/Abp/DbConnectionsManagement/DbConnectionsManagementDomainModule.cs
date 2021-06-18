using Microsoft.Extensions.Caching.Distributed;
using SharpAbp.Abp.DbConnections;
using System;
using Volo.Abp.Caching;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    [DependsOn(
        typeof(AbpDddDomainModule),
        typeof(AbpCachingModule),
        typeof(AbpDbConnectionsModule),
        typeof(DbConnectionsManagementDomainSharedModule)
        )]
    public class DbConnectionsManagementDomainModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpDistributedCacheOptions>(options =>
            {
                options.CacheConfigurators.Add(cacheName =>
                {
                    if (cacheName == CacheNameAttribute.GetCacheName(typeof(DatabaseConnectionInfoCacheItem)))
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
