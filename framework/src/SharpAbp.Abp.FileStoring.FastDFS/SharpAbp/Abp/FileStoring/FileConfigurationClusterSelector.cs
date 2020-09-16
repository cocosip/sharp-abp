using FastDFSCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoring
{
    public class FileConfigurationClusterSelector : IClusterSelector, ISingletonDependency
    {
        private readonly IFileContainerConfigurationProvider _fileContainerConfigurationProvider;

        public FileConfigurationClusterSelector(IFileContainerConfigurationProvider fileContainerConfigurationProvider)
        {
            _fileContainerConfigurationProvider = fileContainerConfigurationProvider;
        }

        public ClusterConfiguration Get(string name)
        {

            bool f(FileContainerConfiguration c)
            {
                if (c.ProviderType == typeof(FastDFSFileProvider))
                {
                    var clusterName = c.GetConfiguration<string>(FastDFSFileProviderConfigurationNames.ClusterName);
                    if (string.Equals(name, clusterName, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                return false;
            }


            var fastDFSFileProviderConfiguration = _fileContainerConfigurationProvider.GetList(f).FirstOrDefault()?.GetFastDFSConfiguration();
            if (fastDFSFileProviderConfiguration != null)
            {

                var clusterConfiguration = new ClusterConfiguration()
                {
                    Name = fastDFSFileProviderConfiguration.ClusterName,
                    Charset = fastDFSFileProviderConfiguration.Charset,
                    ConnectionLifeTime = fastDFSFileProviderConfiguration.ConnectionLifeTime,
                    ConnectionConcurrentThread = fastDFSFileProviderConfiguration.ConnectionConcurrentThread,
                    ScanTimeoutConnectionInterval = fastDFSFileProviderConfiguration.ScanTimeoutConnectionInterval,
                    TrackerMaxConnection = fastDFSFileProviderConfiguration.TrackerMaxConnection,
                    StorageMaxConnection = fastDFSFileProviderConfiguration.StorageMaxConnection,
                    ConnectionTimeout = fastDFSFileProviderConfiguration.ConnectionTimeout,
                    Trackers = fastDFSFileProviderConfiguration.Trackers.ToTrackers()?.ToList() ?? new List<Tracker>()
                };
                return clusterConfiguration;
            }
            return null;
        }



    }
}
