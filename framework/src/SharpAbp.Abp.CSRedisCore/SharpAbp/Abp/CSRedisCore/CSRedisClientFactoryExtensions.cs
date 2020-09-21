using CSRedis;

namespace SharpAbp.Abp.CSRedisCore
{
    public static class CSRedisClientFactoryExtensions
    {
        public static CSRedisClient Get<TContainer>(
            this ICSRedisClientFactory clientFactory
        )
        {
            return clientFactory.Get(
                  CSRedisClientNameAttribute.GetClientName<TContainer>()
            );
        }
    }
}
