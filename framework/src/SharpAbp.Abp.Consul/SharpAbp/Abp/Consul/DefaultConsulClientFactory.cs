using Consul;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Consul
{
    public class DefaultConsulClientFactory : IConsulClientFactory, ISingletonDependency
    {
        private readonly object SyncObject = new object();
        private readonly ConcurrentDictionary<string, IConsulClient> _clientDict;

        protected ILogger Logger { get; }
        protected IConsulConfigurationProvider ConsulConfigurationProvider { get; }
        protected IConsulClientBuilder ConsulClientBuilder { get; }

        public DefaultConsulClientFactory(ILogger<DefaultConsulClientFactory> logger, IConsulConfigurationProvider consulConfigurationProvider, IConsulClientBuilder consulClientBuilder)
        {
            Logger = logger;
            ConsulConfigurationProvider = consulConfigurationProvider;
            ConsulClientBuilder = consulClientBuilder;

            _clientDict = new ConcurrentDictionary<string, IConsulClient>();
        }


        /// <summary>
        /// Get consul client by name
        /// </summary>
        /// <param name="name">name</param>
        /// <returns></returns>
        [NotNull]
        public virtual IConsulClient Get([NotNull] string name = DefaultConsul.Name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            if (!_clientDict.TryGetValue(name, out IConsulClient client))
            {
                var configuration = ConsulConfigurationProvider.Get(name);
                if (configuration == null)
                {
                    throw new AbpException($"Could not find configuration by name '{name}'");
                }

                lock (SyncObject)
                {
                    //Still can't find client
                    if (!_clientDict.TryGetValue(name, out client))
                    {
                        client = ConsulClientBuilder.CreateClient(configuration);
                        if (_clientDict.TryAdd(name, client))
                        {
                            Logger.LogInformation("Create and add consul client '{0}',Address:{1},DataCenter:'{2}'.", name, configuration.Address, configuration.DataCenter);
                        }
                        else
                        {
                            Logger.LogWarning("Add consul client to dict fail! client name:{0}", name);
                        }
                    }
                }
            }

            return client;
        }

    }
}
