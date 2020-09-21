using CSRedis;

namespace SharpAbp.Abp.CSRedisCore
{
    public interface ICSRedisClientBuilder
    {
        CSRedisClient CreateClient(CSRedisConfiguration configuration);
    }
}
