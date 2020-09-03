using Volo.Abp;

namespace SharpAbp.Abp.FileStoring
{
    public class S3FileProviderConfiguration
    {
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
        public string SecretKey
        {
            get => _containerConfiguration.GetConfiguration<string>(S3FileProviderConfigurationNames.AccessKeySecret);
            set => _containerConfiguration.SetConfiguration(S3FileProviderConfigurationNames.AccessKeySecret, Check.NotNullOrWhiteSpace(value, nameof(value)));
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
            get => _containerConfiguration.GetConfigurationOrDefault(S3FileProviderConfigurationNames.Protocol, 1);
            set => _containerConfiguration.SetConfiguration(S3FileProviderConfigurationNames.Protocol, value);
        }



        /// <summary>
        ///  S3 Vendor
        /// </summary>
        public string Vendor
        {
            get => _containerConfiguration.GetConfiguration<string>(S3FileProviderConfigurationNames.Vendor);
            set => _containerConfiguration.SetConfiguration(S3FileProviderConfigurationNames.Vendor, Check.NotNullOrWhiteSpace(value, nameof(value)));
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


        private readonly FileContainerConfiguration _containerConfiguration;

        public S3FileProviderConfiguration(FileContainerConfiguration containerConfiguration)
        {
            _containerConfiguration = containerConfiguration;
        }

    }
}
