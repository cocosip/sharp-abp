using System;
using Volo.Abp;

namespace SharpAbp.Abp.FileStoring.FastDFS
{
    public class FastDFSFileProviderConfiguration
    {
        public string ClusterName
        {
            get => _containerConfiguration.GetConfiguration<string>(FastDFSFileProviderConfigurationNames.ClusterName);
            set => _containerConfiguration.SetConfiguration(FastDFSFileProviderConfigurationNames.ClusterName, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        /// <summary>
        /// Storage group name
        /// </summary>
        public string GroupName
        {
            get => _containerConfiguration.GetConfiguration<string>(FastDFSFileProviderConfigurationNames.GroupName);
            set => _containerConfiguration.SetConfiguration(FastDFSFileProviderConfigurationNames.GroupName, value.IsNullOrWhiteSpace() ? "group1" : value);
        }

        /// <summary>
        /// Access server url
        /// </summary>
        public string HttpServer
        {
            get => _containerConfiguration.GetConfiguration<string>(FastDFSFileProviderConfigurationNames.HttpServer);
            set => _containerConfiguration.SetConfiguration(FastDFSFileProviderConfigurationNames.HttpServer, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        /// <summary>
        /// The url contain group name or not
        /// </summary>
        public bool AppendGroupNameToUrl
        {
            get => _containerConfiguration.GetConfigurationOrDefault(FastDFSFileProviderConfigurationNames.AppendGroupNameToUrl, true);
            set => _containerConfiguration.SetConfiguration(FastDFSFileProviderConfigurationNames.AppendGroupNameToUrl, value);
        }


        /// <summary>
        /// Trackers, 192.168.0.100:22122,192.168.0.101:22122
        /// </summary>
        public string Trackers
        {
            get => _containerConfiguration.GetConfiguration<string>(FastDFSFileProviderConfigurationNames.Trackers);
            set => _containerConfiguration.SetConfiguration(FastDFSFileProviderConfigurationNames.Trackers, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        /// <summary>
        /// AntiStealToken
        /// </summary>
        public bool AntiStealToken
        {
            get => _containerConfiguration.GetConfigurationOrDefault(FastDFSFileProviderConfigurationNames.AntiStealToken, true);
            set => _containerConfiguration.SetConfiguration(FastDFSFileProviderConfigurationNames.AntiStealToken, value);
        }

        /// <summary>
        /// SecretKey, to create access token
        /// </summary>
        public string SecretKey
        {
            get => _containerConfiguration.GetConfiguration<string>(FastDFSFileProviderConfigurationNames.SecretKey);
            set => _containerConfiguration.SetConfiguration(FastDFSFileProviderConfigurationNames.SecretKey, value);
        }

        /// <summary>
        /// ConnectionLifeTime
        /// </summary>
        public int ConnectionLifeTime
        {
            get => _containerConfiguration.GetConfigurationOrDefault(FastDFSFileProviderConfigurationNames.ConnectionLifeTime, 600);
            set => _containerConfiguration.SetConfiguration(FastDFSFileProviderConfigurationNames.ConnectionLifeTime, value);
        }

        /// <summary>
        /// ConnectionTimeout
        /// </summary>
        public int ConnectionTimeout
        {
            get => _containerConfiguration.GetConfigurationOrDefault(FastDFSFileProviderConfigurationNames.ConnectionTimeout, 5);
            set => _containerConfiguration.SetConfiguration(FastDFSFileProviderConfigurationNames.ConnectionTimeout, value);
        }

        /// <summary>
        /// Charset
        /// </summary>
        public string Charset
        {
            get => _containerConfiguration.GetConfiguration<string>(FastDFSFileProviderConfigurationNames.Charset);
            set => _containerConfiguration.SetConfiguration(FastDFSFileProviderConfigurationNames.Charset, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        /// <summary>
        /// ConnectionConcurrentThread
        /// </summary>
        public int ConnectionConcurrentThread
        {
            get => _containerConfiguration.GetConfigurationOrDefault(FastDFSFileProviderConfigurationNames.ConnectionConcurrentThread, 3);
            set => _containerConfiguration.SetConfiguration(FastDFSFileProviderConfigurationNames.ConnectionConcurrentThread, value);
        }


        /// <summary>
        /// ScanTimeoutConnectionInterval
        /// </summary>
        public int ScanTimeoutConnectionInterval
        {
            get => _containerConfiguration.GetConfigurationOrDefault(FastDFSFileProviderConfigurationNames.ScanTimeoutConnectionInterval, 10);
            set => _containerConfiguration.SetConfiguration(FastDFSFileProviderConfigurationNames.ScanTimeoutConnectionInterval, value);
        }

        /// <summary>
        /// TrackerMaxConnection 
        /// </summary>
        public int TrackerMaxConnection
        {
            get => _containerConfiguration.GetConfigurationOrDefault(FastDFSFileProviderConfigurationNames.TrackerMaxConnection, 3);
            set => _containerConfiguration.SetConfiguration(FastDFSFileProviderConfigurationNames.TrackerMaxConnection, value);
        }

        /// <summary>
        /// StorageMaxConnection  
        /// </summary>
        public int StorageMaxConnection
        {
            get => _containerConfiguration.GetConfigurationOrDefault(FastDFSFileProviderConfigurationNames.StorageMaxConnection, 10);
            set => _containerConfiguration.SetConfiguration(FastDFSFileProviderConfigurationNames.StorageMaxConnection, value);
        }


        private readonly FileContainerConfiguration _containerConfiguration;

        public FastDFSFileProviderConfiguration(FileContainerConfiguration containerConfiguration)
        {
            _containerConfiguration = containerConfiguration;
        }
    }
}
