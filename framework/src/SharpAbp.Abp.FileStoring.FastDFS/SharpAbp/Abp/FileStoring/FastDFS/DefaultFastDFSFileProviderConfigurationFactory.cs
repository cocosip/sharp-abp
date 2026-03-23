using FastDFS.Client;
using FastDFS.Client.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoring.FastDFS
{
    public class DefaultFastDFSFileProviderConfigurationFactory : IFastDFSFileProviderConfigurationFactory, ISingletonDependency
    {
        private readonly ILogger _logger;
        private readonly IFastDFSClientFactory _clientFactory;

        private readonly object _syncObject = new object();
        private readonly ConcurrentDictionary<string, FastDFSFileProviderConfiguration> _configurationDict;

        public DefaultFastDFSFileProviderConfigurationFactory(
            ILogger<DefaultFastDFSFileProviderConfigurationFactory> logger,
            IFastDFSClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _configurationDict = new ConcurrentDictionary<string, FastDFSFileProviderConfiguration>();
        }

        public bool AddIfNotContains(FastDFSFileProviderConfiguration configuration)
        {
            if (_configurationDict.ContainsKey(configuration.ClusterName))
            {
                return false;
            }

            lock (_syncObject)
            {
                if (!_configurationDict.TryAdd(configuration.ClusterName, configuration))
                {
                    return false;
                }

                var fastDFSConfig = BuildFastDFSConfiguration(configuration);
                _clientFactory.RegisterClient(configuration.ClusterName, fastDFSConfig);
                return true;
            }
        }

        public FastDFSFileProviderConfiguration GetConfiguration(string name)
        {
            if (!_configurationDict.TryGetValue(name, out FastDFSFileProviderConfiguration? configuration))
            {
                _logger.LogDebug("Can't find FastDFS cluster by name '{Name}'.", name);
            }
            return configuration!;
        }

        private static FastDFSConfiguration BuildFastDFSConfiguration(FastDFSFileProviderConfiguration config)
        {
            var trackerServers = config.Trackers
                .Split(',')
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .ToList();

            var fastDFSConfig = new FastDFSConfiguration
            {
                TrackerServers = trackerServers,
                Charset = config.Charset ?? "UTF-8",
                NetworkTimeout = config.NetworkTimeout,
                ConnectionPool = new ConnectionPoolConfiguration
                {
                    MaxConnectionPerServer = config.MaxConnectionPerServer,
                    MinConnectionPerServer = config.MinConnectionPerServer,
                    ConnectionIdleTimeout = config.ConnectionIdleTimeout,
                    ConnectionLifetime = config.ConnectionLifeTime,
                    ConnectionTimeout = config.ConnectionTimeout,
                    SendTimeout = config.SendTimeout,
                    ReceiveTimeout = config.ReceiveTimeout,
                }
            };

            if (!config.HttpServer.IsNullOrWhiteSpace())
            {
                fastDFSConfig.HttpConfig = new HttpConfiguration
                {
                    ServerUrls = new Dictionary<string, string>
                    {
                        [config.GroupName] = config.HttpServer
                    },
                    SecretKey = config.SecretKey,
                    AntiStealTokenEnabled = config.AntiStealCheckToken,
                    DefaultTokenExpireSeconds = config.DefaultTokenExpireSeconds
                };
            }

            return fastDFSConfig;
        }
    }
}
