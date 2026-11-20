using FreeRedis;
using JetBrains.Annotations;
using System.Collections.Generic;

namespace SharpAbp.Abp.FreeRedis
{
    public interface IRedisClientFactory
    {
        /// <summary>
        /// Gets a shared Redis client by name. The factory owns its lifetime;
        /// callers must not dispose it or use it after the factory shuts down.
        /// </summary>
        /// <param name="name">name</param>
        /// <returns></returns>
        [NotNull]
        RedisClient Get([NotNull] string name = DefaultClient.Name);


        /// <summary>
        /// Gets all initialized shared clients. The factory owns their lifetime.
        /// </summary>
        /// <returns></returns>
        List<RedisClient> GetAll();
    }
}
