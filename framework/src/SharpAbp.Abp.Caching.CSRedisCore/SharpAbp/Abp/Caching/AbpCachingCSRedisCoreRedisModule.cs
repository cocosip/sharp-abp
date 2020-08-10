using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SharpAbp.Abp.CSRedisCore;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Caching
{
    [DependsOn(typeof(AbpCSRedisCoreModule))]
    public class AbpCachingCSRedisCoreRedisModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.Replace(ServiceDescriptor.Singleton<IDistributedCache, AbpRedisCache>());
        }
    }
}
