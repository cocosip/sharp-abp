using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoring.FastDFS
{
    public class DefaultFastDFSFileProviderConfigurationFactory : IFastDFSFileProviderConfigurationFactory, ISingletonDependency
    {
        private readonly ILogger _logger;

        private readonly object syncObject = new object();
        private readonly ConcurrentDictionary<string, FastDFSFileProviderConfiguration> _configurationDict;
        public DefaultFastDFSFileProviderConfigurationFactory(ILogger<DefaultFastDFSFileProviderConfigurationFactory> logger)
        {
            _logger = logger;
            _configurationDict = new ConcurrentDictionary<string, FastDFSFileProviderConfiguration>();
        }


        public bool AddIfNotContains(FastDFSFileProviderConfiguration configuration)
        {
            if (_configurationDict.ContainsKey(configuration.ClusterName))
            {
                return false;
            }

            lock (syncObject)
            {
                return _configurationDict.TryAdd(configuration.ClusterName, configuration);
            }
        }
        
        public FastDFSFileProviderConfiguration GetConfiguration(string name)
        {
            if (!_configurationDict.TryGetValue(name, out FastDFSFileProviderConfiguration configuration))
            {
                _logger.LogDebug("Can't find 'FastDFS' cluster by name '{0}'.", name);
            }
            return configuration;
        }

    }
}
