using CSRedis;

namespace SharpAbp.Abp.CSRedisCore
{
    public interface ICSRedisClientBuilder
    {
        CSRedisClient CreateClient(CSRedisClientConfiguration configuration);
    }
}
