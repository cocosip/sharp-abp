using FastDFSCore;
using System.Collections.Generic;
using System.Linq;

namespace SharpAbp.Abp.FileStoring.FastDFS
{
    public class FileConfigurationClusterSelector : IClusterSelector
    {
        private readonly IFastDFSFileProviderConfigurationFactory _configurationFactory;

        public FileConfigurationClusterSelector(IFastDFSFileProviderConfigurationFactory configurationFactory)
        {
            _configurationFactory = configurationFactory;
        }

        public ClusterConfiguration Get(string name)
        {
            var fastDFSFileProviderConfiguration = _configurationFactory.GetConfiguration(name);
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
