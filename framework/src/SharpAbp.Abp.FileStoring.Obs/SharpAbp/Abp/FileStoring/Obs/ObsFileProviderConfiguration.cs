using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;

namespace SharpAbp.Abp.FileStoring.Obs
{
    public class ObsFileProviderConfiguration
    {
        public string AccessKeyId
        {
            get => _containerConfiguration.GetConfiguration<string>(ObsFileProviderConfigurationNames.AccessKeyId);
            set => _containerConfiguration.SetConfiguration(ObsFileProviderConfigurationNames.AccessKeyId, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        public string AccessKeySecret
        {
            get => _containerConfiguration.GetConfiguration<string>(ObsFileProviderConfigurationNames.AccessKeySecret);
            set => _containerConfiguration.SetConfiguration(ObsFileProviderConfigurationNames.AccessKeySecret, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        public string Endpoint
        {
            get => _containerConfiguration.GetConfiguration<string>(ObsFileProviderConfigurationNames.Endpoint);
            set => _containerConfiguration.SetConfiguration(ObsFileProviderConfigurationNames.Endpoint, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        /// <summary>
        /// Authentication type used to access OBS. Allowed values: OBS, V2, V4.
        /// Default value: OBS.
        /// The SDK ignores this value while <see cref="AuthTypeNegotiation"/> is true,
        /// so set <see cref="AuthTypeNegotiation"/> to false when using V2 or V4.
        /// </summary>
        public string AuthType
        {
            get => _containerConfiguration.GetConfigurationOrDefault(ObsFileProviderConfigurationNames.AuthType, "OBS") ?? "OBS";
            set => _containerConfiguration.SetConfiguration(ObsFileProviderConfigurationNames.AuthType, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        /// <summary>
        /// Whether the SDK negotiates the authentication type with the server.
        /// Default value: true. Negotiation only selects OBS or V2; it never selects V4.
        /// </summary>
        public bool AuthTypeNegotiation
        {
            get => _containerConfiguration.GetConfigurationOrDefault(ObsFileProviderConfigurationNames.AuthTypeNegotiation, true);
            set => _containerConfiguration.SetConfiguration(ObsFileProviderConfigurationNames.AuthTypeNegotiation, value);
        }

        /// <summary>
        /// This name may only contain lowercase letters, numbers, and hyphens, and must begin with a letter or a number.
        /// Each hyphen must be preceded and followed by a non-hyphen character.
        /// The name must also be between 3 and 63 characters long.
        /// If this parameter is not specified, the ContainerName of the <see cref="FileProviderArgs"/> will be used.
        /// </summary>
        public string BucketName
        {
            get => _containerConfiguration.GetConfiguration<string>(ObsFileProviderConfigurationNames.BucketName);
            set => _containerConfiguration.SetConfiguration(ObsFileProviderConfigurationNames.BucketName, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        /// <summary>
        /// Default value: false.
        /// </summary>
        public bool CreateContainerIfNotExists
        {
            get => _containerConfiguration.GetConfigurationOrDefault(ObsFileProviderConfigurationNames.CreateContainerIfNotExists, false);
            set => _containerConfiguration.SetConfiguration(ObsFileProviderConfigurationNames.CreateContainerIfNotExists, value);
        }

        private readonly FileContainerConfiguration _containerConfiguration;

        public ObsFileProviderConfiguration(FileContainerConfiguration containerConfiguration)
        {
            _containerConfiguration = containerConfiguration;
        }
    }
}
