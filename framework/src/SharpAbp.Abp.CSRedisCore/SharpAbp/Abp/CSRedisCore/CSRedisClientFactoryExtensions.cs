using CSRedis;

namespace SharpAbp.Abp.CSRedisCore
{
    public static class CSRedisClientFactoryExtensions
    {
        public static CSRedisClient Get<TClient>(
            this ICSRedisClientFactory clientFactory
        )
        {
            return clientFactory.Get(
                  CSRedisClientNameAttribute.GetClientName<TClient>()
            );
        }
    }
}
