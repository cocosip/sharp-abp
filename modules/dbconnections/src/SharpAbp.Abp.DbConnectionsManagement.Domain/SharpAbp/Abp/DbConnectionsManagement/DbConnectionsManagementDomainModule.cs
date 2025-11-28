using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SharpAbp.Abp.DbConnections;
using Volo.Abp.Caching;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

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
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {

            Configure<AbpDistributedCacheOptions>(options =>
            {
                options.CacheConfigurators.Add(cacheName =>
                {
                    if (cacheName == CacheNameAttribute.GetCacheName(typeof(DatabaseConnectionCacheItem)))
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
