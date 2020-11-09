using FreeRedis;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FreeRedis
{
    public class DefaultRedisClientBuilder : IRedisClientBuilder, ITransientDependency
    {
        public virtual RedisClient CreateClient(FreeRedisConfiguration configuration)
        {
            if (configuration.Mode == RedisMode.Sentinel)
            {
                return new RedisClient(configuration.ConnectionString, configuration.Sentinels.ToArray(), configuration.ReadOnly);
            }
            else
            {
                return new RedisClient(configuration.ConnectionString);
            }
        }
    }
}
