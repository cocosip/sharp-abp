using Volo.Abp;
using KS3SDK = KS3;

namespace SharpAbp.Abp.FileStoring.KS3
{
    public class KS3FileProviderConfiguration
    {
        /// <summary>
        /// This name may only contain lowercase letters, numbers, and hyphens, and must begin with a letter or a number.
        /// Each hyphen must be preceded and followed by a non-hyphen character.
        /// The name must also be between 3 and 63 characters long.
        /// If this parameter is not specified, the ContainerName of the <see cref="FileProviderArgs"/> will be used.
        /// </summary>
        public string BucketName
        {
            get => _containerConfiguration.GetConfiguration<string>(KS3FileProviderConfigurationNames.BucketName);
            set => _containerConfiguration.SetConfiguration(KS3FileProviderConfigurationNames.BucketName, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        /// <summary>
        /// endPoint is an URL, domain name, IPv4 address or IPv6 address.
        /// </summary>
        public string Endpoint
        {
            get => _containerConfiguration.GetConfiguration<string>(KS3FileProviderConfigurationNames.Endpoint);
            set => _containerConfiguration.SetConfiguration(KS3FileProviderConfigurationNames.Endpoint, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        /// <summary>
        /// accessKey is like user-id that uniquely identifies your account.This field is optional and can be omitted for anonymous access.
        /// </summary>
        public string AccessKey
        {
            get => _containerConfiguration.GetConfiguration<string>(KS3FileProviderConfigurationNames.AccessKey);
            set => _containerConfiguration.SetConfiguration(KS3FileProviderConfigurationNames.AccessKey, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        /// <summary>
        /// secretKey is the password to your account.This field is optional and can be omitted for anonymous access.
        /// </summary>
        public string SecretKey
        {
            get => _containerConfiguration.GetConfiguration<string>(KS3FileProviderConfigurationNames.SecretKey);
            set => _containerConfiguration.SetConfiguration(KS3FileProviderConfigurationNames.SecretKey, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        /// <summary>
        /// http Or https
        /// </summary>
        public string? Protocol
        {
            get => _containerConfiguration.GetConfigurationOrDefault<string>(KS3FileProviderConfigurationNames.Protocol, KS3SDK.Http.Protocol.HTTP);
            set => _containerConfiguration.SetConfiguration(KS3FileProviderConfigurationNames.Protocol, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        /// <summary>
        /// User agent
        /// </summary>
        public string? UserAgent
        {
            get => _containerConfiguration.GetConfigurationOrDefault(KS3FileProviderConfigurationNames.UserAgent, KS3SDK.ClientConfiguration.DEFAULT_USER_AGENT);
            set => _containerConfiguration.SetConfiguration(KS3FileProviderConfigurationNames.UserAgent, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        /// <summary>
        /// Max connection
        /// </summary>
        public int MaxConnections
        {
            get => _containerConfiguration.GetConfigurationOrDefault(KS3FileProviderConfigurationNames.MaxConnections, KS3SDK.ClientConfiguration.DEFAULT_MAX_CONNECTIONS);
            set => _containerConfiguration.SetConfiguration(KS3FileProviderConfigurationNames.MaxConnections, value);
        }

        /// <summary>
        /// Timeout
        /// </summary>
        public int Timeout
        {
            get => _containerConfiguration.GetConfigurationOrDefault(KS3FileProviderConfigurationNames.Timeout, KS3SDK.ClientConfiguration.DEFAULT_TIMEOUT);
            set => _containerConfiguration.SetConfiguration(KS3FileProviderConfigurationNames.Timeout, value);
        }

        /// <summary>
        /// ReadWrite timeout
        /// </summary>
        public int ReadWriteTimeout
        {
            get => _containerConfiguration.GetConfigurationOrDefault(KS3FileProviderConfigurationNames.ReadWriteTimeout, KS3SDK.ClientConfiguration.DEFAULT_READ_WRITE_TIMEOUT);
            set => _containerConfiguration.SetConfiguration(KS3FileProviderConfigurationNames.ReadWriteTimeout, value);
        }


        /// <summary>
        /// Default value: false.
        /// </summary>
        public bool CreateContainerIfNotExists
        {
            get => _containerConfiguration.GetConfigurationOrDefault(KS3FileProviderConfigurationNames.CreateContainerIfNotExists, false);
            set => _containerConfiguration.SetConfiguration(KS3FileProviderConfigurationNames.CreateContainerIfNotExists, value);
        }


        private readonly FileContainerConfiguration _containerConfiguration;

        public KS3FileProviderConfiguration(FileContainerConfiguration containerConfiguration)
        {
            _containerConfiguration = containerConfiguration;
        }
    }
}
