using FastDFSCore;
using System;
using System.Collections.Generic;
using Volo.Abp;

namespace SharpAbp.Abp.FileStoring
{
    public class FastDFSFileProviderConfiguration
    {
        public string ClusterName
        {
            get => _containerConfiguration.GetConfiguration<string>(FastDFSFileProviderConfigurationNames.ClusterName);
            set => _containerConfiguration.SetConfiguration(FastDFSFileProviderConfigurationNames.ClusterName, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        /// <summary>
        /// storage group name of fastdfs
        /// </summary>
        public string GroupName
        {
            get => _containerConfiguration.GetConfiguration<string>(FastDFSFileProviderConfigurationNames.GroupName);
            set => _containerConfiguration.SetConfiguration(FastDFSFileProviderConfigurationNames.GroupName, value.IsNullOrWhiteSpace() ? "group1" : value);
        }

        /// <summary>
        /// Trackers
        /// </summary>
        public List<Tracker> Trackers
        {
            get => _containerConfiguration.GetConfigurationOrDefault(FastDFSFileProviderConfigurationNames.Trackers, new List<Tracker>());
            set => _containerConfiguration.SetConfiguration(FastDFSFileProviderConfigurationNames.Trackers, value);
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
        /// ConnectionLifeTime
        /// </summary>
        public int ConnectionLifeTime
        {
            get => _containerConfiguration.GetConfigurationOrDefault(FastDFSFileProviderConfigurationNames.ConnectionLifeTime, 600);
            set => _containerConfiguration.SetConfiguration(FastDFSFileProviderConfigurationNames.ConnectionLifeTime, value);
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
