using FreeRedis;
using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.FreeRedis
{
    /// <summary>
    /// Configuration class for FreeRedis client.
    /// </summary>
    public class FreeRedisConfiguration
    {
        /// <summary>
        /// Gets or sets the Redis mode.
        /// </summary>
        public RedisMode Mode { get; set; }

        /// <summary>
        /// Gets or sets the connection string for Redis.
        /// </summary>
        public string? ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the list of sentinel addresses for Redis Sentinel mode.
        /// </summary>
        public List<string> Sentinels { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the connection is read-only.
        /// </summary>
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Gets or sets the list of client configuration actions.
        /// </summary>
        public List<Action<RedisClient>> CliConfigures { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FreeRedisConfiguration"/> class.
        /// </summary>
        public FreeRedisConfiguration()
        {
            Sentinels = [];
            CliConfigures = [];
        }
    }
}