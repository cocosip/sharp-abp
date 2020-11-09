using FreeRedis;

namespace SharpAbp.Abp.FreeRedis
{
    public interface IRedisClientBuilder
    {
        RedisClient CreateClient(FreeRedisConfiguration configuration);
    }
}
