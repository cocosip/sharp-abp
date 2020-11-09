using FreeRedis;
using JetBrains.Annotations;
using System.Collections.Generic;

namespace SharpAbp.Abp.FreeRedis
{
    public interface IRedisClientFactory
    {
        /// <summary>
        /// Get csredis client by name
        /// </summary>
        /// <param name="name">name</param>
        /// <returns></returns>
        [NotNull]
        RedisClient Get([NotNull] string name = DefaultClient.Name);


        /// <summary>
        /// Get all csredis client
        /// </summary>
        /// <returns></returns>
        List<RedisClient> GetAll();
    }
}
