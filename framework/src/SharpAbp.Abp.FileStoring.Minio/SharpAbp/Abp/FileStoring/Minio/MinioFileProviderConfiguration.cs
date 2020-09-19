using Volo.Abp;

namespace SharpAbp.Abp.FileStoring.Minio
{
    public class MinioFileProviderConfiguration
    {
        public string BucketName
        {
            get => _containerConfiguration.GetConfiguration<string>(MinioFileProviderConfigurationNames.BucketName);
            set => _containerConfiguration.SetConfiguration(MinioFileProviderConfigurationNames.BucketName, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        /// <summary>
        /// endPoint is an URL, domain name, IPv4 address or IPv6 address.
        /// </summary>
        public string EndPoint
        {
            get => _containerConfiguration.GetConfiguration<string>(MinioFileProviderConfigurationNames.EndPoint);
            set => _containerConfiguration.SetConfiguration(MinioFileProviderConfigurationNames.EndPoint, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        /// <summary>
        /// accessKey is like user-id that uniquely identifies your account.This field is optional and can be omitted for anonymous access.
        /// </summary>
        public string AccessKey
        {
            get => _containerConfiguration.GetConfiguration<string>(MinioFileProviderConfigurationNames.AccessKey);
            set => _containerConfiguration.SetConfiguration(MinioFileProviderConfigurationNames.AccessKey, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        /// <summary>
        /// secretKey is the password to your account.This field is optional and can be omitted for anonymous access.
        /// </summary>
        public string SecretKey
        {
            get => _containerConfiguration.GetConfiguration<string>(MinioFileProviderConfigurationNames.SecretKey);
            set => _containerConfiguration.SetConfiguration(MinioFileProviderConfigurationNames.SecretKey, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        /// <summary>
        ///connect to  to MinIO Client object to use https instead of http
        /// </summary>
        public bool WithSSL
        {
            get => _containerConfiguration.GetConfigurationOrDefault(MinioFileProviderConfigurationNames.WithSSL, false);
            set => _containerConfiguration.SetConfiguration(MinioFileProviderConfigurationNames.WithSSL, value);
        }

        /// <summary>
        ///Default value: false.
        /// </summary>
        public bool CreateBucketIfNotExists
        {
            get => _containerConfiguration.GetConfigurationOrDefault(MinioFileProviderConfigurationNames.CreateBucketIfNotExists, false);
            set => _containerConfiguration.SetConfiguration(MinioFileProviderConfigurationNames.CreateBucketIfNotExists, value);
        }

        private readonly FileContainerConfiguration _containerConfiguration;

        public MinioFileProviderConfiguration(FileContainerConfiguration containerConfiguration)
        {
            _containerConfiguration = containerConfiguration;
        }
    }
}
