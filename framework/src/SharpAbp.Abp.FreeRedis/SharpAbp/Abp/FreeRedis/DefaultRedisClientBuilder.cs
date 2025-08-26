using FreeRedis;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FreeRedis
{
    /// <summary>
    /// Default implementation of <see cref="IRedisClientBuilder"/> for creating Redis clients.
    /// </summary>
    public class DefaultRedisClientBuilder : IRedisClientBuilder, ITransientDependency
    {
        /// <summary>
        /// Creates a Redis client based on the provided configuration.
        /// </summary>
        /// <param name="configuration">The Redis configuration.</param>
        /// <returns>A configured Redis client instance.</returns>
        public virtual RedisClient CreateClient(FreeRedisConfiguration configuration)
        {
            RedisClient cli;
            if (configuration.Mode == RedisMode.Sentinel)
            {
                cli = new RedisClient(configuration.ConnectionString, [.. configuration.Sentinels], configuration.ReadOnly);
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