using FreeRedis;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FreeRedis
{
    public class DefaultRedisClientBuilder : IRedisClientBuilder, ITransientDependency
    {
        public virtual RedisClient CreateClient(FreeRedisConfiguration configuration)
        {
            RedisClient cli;
            if (configuration.Mode == RedisMode.Sentinel)
            {
                cli = new RedisClient(configuration.ConnectionString, configuration.Sentinels.ToArray(), configuration.ReadOnly);
            }
            else
            {
                cli = new RedisClient(configuration.ConnectionString);
            }

            if (configuration.CliConfigures != null)
            {
                foreach (var c in configuration.CliConfigures)
                {
                    c(cli);
                }
            }

            return cli;
        }
    }
}
