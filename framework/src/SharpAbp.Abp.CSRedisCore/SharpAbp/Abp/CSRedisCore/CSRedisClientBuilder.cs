using CSRedis;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.CSRedisCore
{
    public class CSRedisClientBuilder : ICSRedisClientBuilder, ISingletonDependency
    {
        public virtual CSRedisClient CreateClient(CSRedisClientConfiguration configuration)
        {
            if (configuration.Mode == RedisMode.Sentinel)
            {
                return new CSRedisClient(configuration.ConnectionString, configuration.Sentinels.ToArray(), configuration.ReadOnly);
            }
            else
            {
                return new CSRedisClient(configuration.ConnectionString);
            }
        }

    }
}
