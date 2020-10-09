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
    public class DefaultCSRedisClientFactory : ICSRedisClientFactory, ISingletonDependency
    {
        private readonly object SyncObject = new object();
        private readonly ConcurrentDictionary<string, CSRedisClient> _clientDict;

        protected ILogger Logger { get; }
        protected ICSRedisConfigurationProvider ConfigurationSelector { get; }
        protected ICSRedisClientBuilder ClientBuilder { get; }
        public DefaultCSRedisClientFactory(ILogger<DefaultCSRedisClientFactory> logger, ICSRedisConfigurationProvider configurationSelector, ICSRedisClientBuilder clientBuilder)
        {
            Logger = logger;
            ConfigurationSelector = configurationSelector;
            ClientBuilder = clientBuilder;

            _clientDict = new ConcurrentDictionary<string, CSRedisClient>();
        }

        /// <summary>
        /// Get csredis client by name
        /// </summary>
        /// <param name="name">name</param>
        /// <returns></returns>
        [NotNull]
        public virtual CSRedisClient Get([NotNull] string name = DefaultClient.Name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            if (!_clientDict.TryGetValue(name, out CSRedisClient client))
            {
                var configuration = ConfigurationSelector.Get(name);
                if (configuration == null)
                {
                    throw new AbpException($"Could not find configuration by name '{name}'");
                }

                lock (SyncObject)
                {
                    //Still can't find client
                    if (!_clientDict.TryGetValue(name, out client))
                    {
                        client = ClientBuilder.CreateClient(configuration);
                        if (_clientDict.TryAdd(name, client))
                        {
                            Logger.LogInformation("Create and add csredis client '{0}',ConnectionString:'{1}',Mode:'{2}'.", name, configuration.ConnectionString, configuration.Mode);
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
        public List<CSRedisClient> GetAll()
        {
            return _clientDict.Values.ToList();
        }


    }
}
