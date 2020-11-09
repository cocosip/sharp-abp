using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.CSRedisCore;

namespace SharpAbp.Abp.Caching
{
    //[DisableConventionalRegistration]
    public class AbpRedisCache : CSRedisCache
    {
        public AbpRedisCache(IOptions<CSRedisCoreCacheOptions> options, ICSRedisClientFactory cSRedisClientFactory) : base(cSRedisClientFactory.Get(options.Value.Name))
        {

        }
    }
}
