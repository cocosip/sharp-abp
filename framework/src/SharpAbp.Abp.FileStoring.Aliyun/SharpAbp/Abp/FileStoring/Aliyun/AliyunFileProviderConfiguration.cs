using System;
using Volo.Abp;

namespace SharpAbp.Abp.FileStoring.Aliyun
{
    /// <summary>
    /// Sub-account access to OSS or STS temporary authorization to access OSS
    /// </summary>
    public class AliyunFileProviderConfiguration
    {
        public string AccessKeyId
        {
            get => _containerConfiguration.GetConfiguration<string>(AliyunFileProviderConfigurationNames.AccessKeyId);
            set => _containerConfiguration.SetConfiguration(AliyunFileProviderConfigurationNames.AccessKeyId, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        public string AccessKeySecret
        {
            get => _containerConfiguration.GetConfiguration<string>(AliyunFileProviderConfigurationNames.AccessKeySecret);
            set => _containerConfiguration.SetConfiguration(AliyunFileProviderConfigurationNames.AccessKeySecret, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        public string Endpoint
        {
            get => _containerConfiguration.GetConfiguration<string>(AliyunFileProviderConfigurationNames.Endpoint);
            set => _containerConfiguration.SetConfiguration(AliyunFileProviderConfigurationNames.Endpoint, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        public bool UseSecurityTokenService
        {
            get => _containerConfiguration.GetConfigurationOrDefault(AliyunFileProviderConfigurationNames.UseSecurityTokenService, false);
            set => _containerConfiguration.SetConfiguration(AliyunFileProviderConfigurationNames.UseSecurityTokenService, value);
        }

        public string RegionId
        {
            get => _containerConfiguration.GetConfiguration<string>(AliyunFileProviderConfigurationNames.RegionId);
            set => _containerConfiguration.SetConfiguration(AliyunFileProviderConfigurationNames.RegionId, value);
        }

        /// <summary>
        /// acs:ram::$accountID:role/$roleName
        /// </summary>
        public string RoleArn
        {
            get => _containerConfiguration.GetConfiguration<string>(AliyunFileProviderConfigurationNames.RoleArn);
            set => _containerConfiguration.SetConfiguration(AliyunFileProviderConfigurationNames.RoleArn, value);
        }

        /// <summary>
        /// The name used to identify the temporary access credentials, it is recommended to use different application users to distinguish.
        /// </summary>
        public string RoleSessionName
        {
            get => _containerConfiguration.GetConfiguration<string>(AliyunFileProviderConfigurationNames.RoleSessionName);
            set => _containerConfiguration.SetConfiguration(AliyunFileProviderConfigurationNames.RoleSessionName, value);
        }

        /// <summary>
        /// Set the validity period of the temporary access credential, the unit is s, the minimum is 900, and the maximum is 3600.
        /// </summary>
        public int DurationSeconds
        {
            get => _containerConfiguration.GetConfigurationOrDefault(AliyunFileProviderConfigurationNames.DurationSeconds, 0);
            set => _containerConfiguration.SetConfiguration(AliyunFileProviderConfigurationNames.DurationSeconds, value);
        }

        /// <summary>
        /// If policy is empty, the user will get all permissions under this role
        /// </summary>
        public string Policy
        {
            get => _containerConfiguration.GetConfiguration<string>(AliyunFileProviderConfigurationNames.Policy);
            set => _containerConfiguration.SetConfiguration(AliyunFileProviderConfigurationNames.Policy, value);
        }

        /// <summary>
        /// This name may only contain lowercase letters, numbers, and hyphens, and must begin with a letter or a number.
        /// Each hyphen must be preceded and followed by a non-hyphen character.
        /// The name must also be between 3 and 63 characters long.
        /// If this parameter is not specified, the ContainerName of the <see cref="FileProviderArgs"/> will be used.
        /// </summary>
        public string BucketName
        {
            get => _containerConfiguration.GetConfiguration<string>(AliyunFileProviderConfigurationNames.BucketName);
            set => _containerConfiguration.SetConfiguration(AliyunFileProviderConfigurationNames.BucketName, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        /// <summary>
        /// Default value: false.
        /// </summary>
        public bool CreateContainerIfNotExists
        {
            get => _containerConfiguration.GetConfigurationOrDefault(AliyunFileProviderConfigurationNames.CreateContainerIfNotExists, false);
            set => _containerConfiguration.SetConfiguration(AliyunFileProviderConfigurationNames.CreateContainerIfNotExists, value);
        }

        private readonly string _temporaryCredentialsCacheKey;
        public string? TemporaryCredentialsCacheKey
        {
            get => _containerConfiguration.GetConfigurationOrDefault(AliyunFileProviderConfigurationNames.TemporaryCredentialsCacheKey, _temporaryCredentialsCacheKey);
            set => _containerConfiguration.SetConfiguration(AliyunFileProviderConfigurationNames.TemporaryCredentialsCacheKey, value);
        }

        private readonly  FileContainerConfiguration _containerConfiguration;

        public AliyunFileProviderConfiguration(FileContainerConfiguration containerConfiguration)
        {
            _containerConfiguration = containerConfiguration;
            _temporaryCredentialsCacheKey = Guid.NewGuid().ToString("N");
        }
    }
}
