using FreeRedis;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FreeRedis
{
    public class DefaultRedisClientFactory : IRedisClientFactory, ISingletonDependency
    {
        private readonly object _sync = new object();
        private readonly ConcurrentDictionary<string, RedisClient> _clientDict;

        protected ILogger Logger { get; }
        protected IRedisConfigurationProvider ConfigurationSelector { get; }
        protected IRedisClientBuilder ClientBuilder { get; }
        public DefaultRedisClientFactory(ILogger<DefaultRedisClientFactory> logger, IRedisConfigurationProvider configurationSelector, IRedisClientBuilder clientBuilder)
        {
            Logger = logger;
            ConfigurationSelector = configurationSelector;
            ClientBuilder = clientBuilder;

            _clientDict = new ConcurrentDictionary<string, RedisClient>();
        }

        /// <summary>
        /// Get redis client by name
        /// </summary>
        /// <param name="name">name</param>
        /// <returns></returns>
        [NotNull]
        public virtual RedisClient Get([NotNull] string name = DefaultClient.Name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            if (!_clientDict.TryGetValue(name, out RedisClient client))
            {
                var configuration = ConfigurationSelector.Get(name);
                if (configuration == null)
                {
                    throw new AbpException($"Could not find configuration by name '{name}'");
                }

                lock (_sync)
                {
                    //Still can't find client
                    if (!_clientDict.TryGetValue(name, out client))
                    {
                        client = ClientBuilder.CreateClient(configuration);
                        if (_clientDict.TryAdd(name, client))
                        {
                            Logger.LogInformation("Create and add redis client '{0}',ConnectionString:'{1}',Mode:'{2}'.", name, configuration.ConnectionString, configuration.Mode);
                        }
                        else
                        {
                            Logger.LogWarning("Add client to dict fail! client name:{0}", name);
                        }
                    }
                }
            }

            return client;
        }

        /// <summary>
        /// Get all csredis client
        /// </summary>
        /// <returns></returns>
        public List<RedisClient> GetAll()
        {
            return _clientDict.Values.ToList();
        }

    }
}
