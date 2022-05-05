using Volo.Abp;

namespace SharpAbp.Abp.FileStoring.S3
{
    public class S3FileProviderConfiguration
    {
        public string BucketName
        {
            get => _containerConfiguration.GetConfiguration<string>(S3FileProviderConfigurationNames.BucketName);
            set => _containerConfiguration.SetConfiguration(S3FileProviderConfigurationNames.BucketName, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        /// <summary>
        /// the server url to access
        /// </summary>
        public string ServerUrl
        {
            get => _containerConfiguration.GetConfiguration<string>(S3FileProviderConfigurationNames.ServerUrl);
            set => _containerConfiguration.SetConfiguration(S3FileProviderConfigurationNames.ServerUrl, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        /// <summary>
        /// accessKey is like user-id that uniquely identifies your account.This field is optional and can be omitted for anonymous access.
        /// </summary>
        public string AccessKeyId
        {
            get => _containerConfiguration.GetConfiguration<string>(S3FileProviderConfigurationNames.AccessKeyId);
            set => _containerConfiguration.SetConfiguration(S3FileProviderConfigurationNames.AccessKeyId, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        /// <summary>
        /// secretKey is the password to your account.This field is optional and can be omitted for anonymous access.
        /// </summary>
        public string SecretAccessKey
        {
            get => _containerConfiguration.GetConfiguration<string>(S3FileProviderConfigurationNames.SecretAccessKey);
            set => _containerConfiguration.SetConfiguration(S3FileProviderConfigurationNames.SecretAccessKey, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        /// <summary>
        /// ForcePathStyle.
        /// </summary>
        public bool ForcePathStyle
        {
            get => _containerConfiguration.GetConfigurationOrDefault(S3FileProviderConfigurationNames.ForcePathStyle, false);
            set => _containerConfiguration.SetConfiguration(S3FileProviderConfigurationNames.ForcePathStyle, value);
        }


        /// <summary>
        /// UseChunkEncoding.
        /// </summary>
        public bool UseChunkEncoding
        {
            get => _containerConfiguration.GetConfigurationOrDefault(S3FileProviderConfigurationNames.UseChunkEncoding, false);
            set => _containerConfiguration.SetConfiguration(S3FileProviderConfigurationNames.UseChunkEncoding, value);
        }


        /// <summary>
        /// Protocol.
        /// </summary>
        public int Protocol
        {
            get => _containerConfiguration.GetConfigurationOrDefault(S3FileProviderConfigurationNames.Protocol, (int)Amazon.S3.Protocol.HTTP);
            set => _containerConfiguration.SetConfiguration(S3FileProviderConfigurationNames.Protocol, value);
        }

        /// <summary>
        /// EnableSlice.
        /// </summary>
        public bool EnableSlice
        {
            get => _containerConfiguration.GetConfigurationOrDefault(S3FileProviderConfigurationNames.EnableSlice, true);
            set => _containerConfiguration.SetConfiguration(S3FileProviderConfigurationNames.EnableSlice, value);
        }

        /// <summary>
        /// SliceSize (Default is 5MB)
        /// </summary>
        public int SliceSize
        {
            get => _containerConfiguration.GetConfigurationOrDefault(S3FileProviderConfigurationNames.SliceSize, 5242880);
            set => _containerConfiguration.SetConfiguration(S3FileProviderConfigurationNames.SliceSize, value);
        }

        /// <summary>
        ///  SignatureVersion
        /// </summary>
        public string SignatureVersion
        {
            get => _containerConfiguration.GetConfiguration<string>(S3FileProviderConfigurationNames.SignatureVersion);
            set => _containerConfiguration.SetConfiguration(S3FileProviderConfigurationNames.SignatureVersion, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        /// <summary>
        ///Default value: false.
        /// </summary>
        public bool CreateBucketIfNotExists
        {
            get => _containerConfiguration.GetConfigurationOrDefault(S3FileProviderConfigurationNames.CreateBucketIfNotExists, false);
            set => _containerConfiguration.SetConfiguration(S3FileProviderConfigurationNames.CreateBucketIfNotExists, value);
        }


        private readonly FileContainerConfiguration _containerConfiguration;

        public S3FileProviderConfiguration(FileContainerConfiguration containerConfiguration)
        {
            _containerConfiguration = containerConfiguration;
        }

    }
}
