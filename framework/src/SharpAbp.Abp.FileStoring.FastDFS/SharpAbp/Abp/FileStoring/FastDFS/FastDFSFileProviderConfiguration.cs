using System;
using Volo.Abp;

namespace SharpAbp.Abp.FileStoring.FastDFS
{
    public class FastDFSFileProviderConfiguration
    {
        /// <summary>
        /// Cluster name, used as the named client key in IFastDFSClientFactory.
        /// </summary>
        public string ClusterName
        {
            get => _containerConfiguration.GetConfiguration<string>(FastDFSFileProviderConfigurationNames.ClusterName);
            set => _containerConfiguration.SetConfiguration(FastDFSFileProviderConfigurationNames.ClusterName, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        /// <summary>
        /// Storage group name (e.g. "group1").
        /// </summary>
        public string GroupName
        {
            get => _containerConfiguration.GetConfiguration<string>(FastDFSFileProviderConfigurationNames.GroupName);
            set => _containerConfiguration.SetConfiguration(FastDFSFileProviderConfigurationNames.GroupName, value.IsNullOrWhiteSpace() ? "group1" : value);
        }

        /// <summary>
        /// HTTP access server URL for file URL generation (e.g. "http://192.168.0.100:8080").
        /// Required when HttpAccess is enabled.
        /// </summary>
        public string HttpServer
        {
            get => _containerConfiguration.GetConfiguration<string>(FastDFSFileProviderConfigurationNames.HttpServer);
            set => _containerConfiguration.SetConfiguration(FastDFSFileProviderConfigurationNames.HttpServer, value);
        }

        /// <summary>
        /// Tracker server endpoints, comma-separated (e.g. "192.168.0.100:22122,192.168.0.101:22122").
        /// </summary>
        public string Trackers
        {
            get => _containerConfiguration.GetConfiguration<string>(FastDFSFileProviderConfigurationNames.Trackers);
            set => _containerConfiguration.SetConfiguration(FastDFSFileProviderConfigurationNames.Trackers, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        /// <summary>
        /// Enable anti-steal token validation for HTTP access. Default: false.
        /// </summary>
        public bool AntiStealCheckToken
        {
            get => _containerConfiguration.GetConfigurationOrDefault(FastDFSFileProviderConfigurationNames.AntiStealCheckToken, false);
            set => _containerConfiguration.SetConfiguration(FastDFSFileProviderConfigurationNames.AntiStealCheckToken, value);
        }

        /// <summary>
        /// Secret key for anti-steal token generation. Must match FastDFS Nginx module config.
        /// </summary>
        public string SecretKey
        {
            get => _containerConfiguration.GetConfiguration<string>(FastDFSFileProviderConfigurationNames.SecretKey);
            set => _containerConfiguration.SetConfiguration(FastDFSFileProviderConfigurationNames.SecretKey, value);
        }

        /// <summary>
        /// Default token expiration time in seconds. Default: 3600 (1 hour).
        /// </summary>
        public int DefaultTokenExpireSeconds
        {
            get => _containerConfiguration.GetConfigurationOrDefault(FastDFSFileProviderConfigurationNames.DefaultTokenExpireSeconds, 3600);
            set => _containerConfiguration.SetConfiguration(FastDFSFileProviderConfigurationNames.DefaultTokenExpireSeconds, value);
        }

        /// <summary>
        /// Charset encoding name. Default: "UTF-8".
        /// </summary>
        public string Charset
        {
            get => _containerConfiguration.GetConfiguration<string>(FastDFSFileProviderConfigurationNames.Charset);
            set => _containerConfiguration.SetConfiguration(FastDFSFileProviderConfigurationNames.Charset, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        /// <summary>
        /// General network timeout in seconds. Default: 30.
        /// </summary>
        public int NetworkTimeout
        {
            get => _containerConfiguration.GetConfigurationOrDefault(FastDFSFileProviderConfigurationNames.NetworkTimeout, 30);
            set => _containerConfiguration.SetConfiguration(FastDFSFileProviderConfigurationNames.NetworkTimeout, value);
        }

        /// <summary>
        /// Maximum connections per server in the connection pool. Default: 50.
        /// </summary>
        public int MaxConnectionPerServer
        {
            get => _containerConfiguration.GetConfigurationOrDefault(FastDFSFileProviderConfigurationNames.MaxConnectionPerServer, 50);
            set => _containerConfiguration.SetConfiguration(FastDFSFileProviderConfigurationNames.MaxConnectionPerServer, value);
        }

        /// <summary>
        /// Minimum connections per server (pre-warmed). Default: 5.
        /// </summary>
        public int MinConnectionPerServer
        {
            get => _containerConfiguration.GetConfigurationOrDefault(FastDFSFileProviderConfigurationNames.MinConnectionPerServer, 5);
            set => _containerConfiguration.SetConfiguration(FastDFSFileProviderConfigurationNames.MinConnectionPerServer, value);
        }

        /// <summary>
        /// Connection idle timeout in seconds. Idle connections are closed after this period. Default: 300 (5 min).
        /// </summary>
        public int ConnectionIdleTimeout
        {
            get => _containerConfiguration.GetConfigurationOrDefault(FastDFSFileProviderConfigurationNames.ConnectionIdleTimeout, 300);
            set => _containerConfiguration.SetConfiguration(FastDFSFileProviderConfigurationNames.ConnectionIdleTimeout, value);
        }

        /// <summary>
        /// Maximum connection lifetime in seconds. Default: 3600 (1 hour). 0 = no limit.
        /// </summary>
        public int ConnectionLifeTime
        {
            get => _containerConfiguration.GetConfigurationOrDefault(FastDFSFileProviderConfigurationNames.ConnectionLifeTime, 3600);
            set => _containerConfiguration.SetConfiguration(FastDFSFileProviderConfigurationNames.ConnectionLifeTime, value);
        }

        /// <summary>
        /// Connection timeout in milliseconds. Default: 30000 (30 seconds).
        /// </summary>
        public int ConnectionTimeout
        {
            get => _containerConfiguration.GetConfigurationOrDefault(FastDFSFileProviderConfigurationNames.ConnectionTimeout, 30000);
            set => _containerConfiguration.SetConfiguration(FastDFSFileProviderConfigurationNames.ConnectionTimeout, value);
        }

        /// <summary>
        /// Send timeout in milliseconds. Default: 30000 (30 seconds).
        /// </summary>
        public int SendTimeout
        {
            get => _containerConfiguration.GetConfigurationOrDefault(FastDFSFileProviderConfigurationNames.SendTimeout, 30000);
            set => _containerConfiguration.SetConfiguration(FastDFSFileProviderConfigurationNames.SendTimeout, value);
        }

        /// <summary>
        /// Receive timeout in milliseconds. Default: 30000 (30 seconds).
        /// </summary>
        public int ReceiveTimeout
        {
            get => _containerConfiguration.GetConfigurationOrDefault(FastDFSFileProviderConfigurationNames.ReceiveTimeout, 30000);
            set => _containerConfiguration.SetConfiguration(FastDFSFileProviderConfigurationNames.ReceiveTimeout, value);
        }

        private readonly FileContainerConfiguration _containerConfiguration;

        public FastDFSFileProviderConfiguration(FileContainerConfiguration containerConfiguration)
        {
            _containerConfiguration = containerConfiguration;
        }
    }
}
