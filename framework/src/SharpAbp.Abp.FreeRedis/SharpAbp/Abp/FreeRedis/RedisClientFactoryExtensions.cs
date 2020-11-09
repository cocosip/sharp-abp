using FreeRedis;

namespace SharpAbp.Abp.FreeRedis
{
    public static class RedisClientFactoryExtensions
    {
        public static RedisClient Get<TClient>(this IRedisClientFactory clientFactory)
        {
            return clientFactory.Get(
                  RedisClientNameAttribute.GetClientName<TClient>()
            );
        }
    }
}
