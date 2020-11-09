using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SharpAbp.Abp.FreeRedis;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Caching.FreeRedis
{
    [DependsOn(
        typeof(AbpFreeRedisModule)
    )]
    public class AbpCachingFreeRedisModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.Replace(ServiceDescriptor.Singleton<IDistributedCache, AbpFreeRedisCache>());
        }
    }
}
