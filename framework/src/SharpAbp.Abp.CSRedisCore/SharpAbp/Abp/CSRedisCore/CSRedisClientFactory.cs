using CSRedis;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.CSRedisCore
{
    public class CSRedisClientFactory : ICSRedisClientFactory, ISingletonDependency
    {
        private readonly object _syncObject = new object();
        private readonly ConcurrentDictionary<string, CSRedisClient> _clientDict;

        private readonly ILogger _logger;
        private readonly ICSRedisClientConfigurationSelector _cSRedisClientConfigurationSelector;
        private readonly ICSRedisClientBuilder _cSRedisClientBuilder;
        public CSRedisClientFactory(ILogger<CSRedisClientFactory> logger, ICSRedisClientConfigurationSelector cSRedisClientConfigurationSelector, ICSRedisClientBuilder cSRedisClientBuilder)
        {
            _logger = logger;
            _cSRedisClientConfigurationSelector = cSRedisClientConfigurationSelector;
            _cSRedisClientBuilder = cSRedisClientBuilder;

            _clientDict = new ConcurrentDictionary<string, CSRedisClient>();
        }

        /// <summary>
        /// Get csredis client by name
        /// </summary>
        /// <param name="name">name</param>
        /// <returns></returns>
        [NotNull]
        public virtual CSRedisClient Get([NotNull] string name)
        {

            if (!_clientDict.TryGetValue(name, out CSRedisClient client))
            {
                var configuration = _cSRedisClientConfigurationSelector.Get(name);
                if (configuration == null)
                {
                    throw new AbpException($"Could not find configuration by name '{name}'");
                }

                lock (_syncObject)
                {
                    //Still can't find client
                    if (_clientDict.TryGetValue(name, out client))
                    {
                        client = _cSRedisClientBuilder.CreateClient(configuration);
                        if (_clientDict.TryAdd(name, client))
                        {
                            _logger.LogInformation("Create and add csredis client '{0}',ConnectionString:'{1}',Mode:'{2}'.", name, configuration.ConnectionString, configuration.Mode);
                        }
                        else
                        {
                            _logger.LogWarning("Add client to dict fail! client name:{0}", name);
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
        public List<CSRedisClient> GetAllClients()
        {
            return _clientDict.Values.ToList();
        }


    }
}
