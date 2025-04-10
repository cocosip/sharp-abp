using System;
using Volo.Abp;

namespace SharpAbp.Abp.FileStoring.Aws
{
    public class AwsFileProviderConfiguration
    {
        public string AccessKeyId
        {
            get => _containerConfiguration.GetConfiguration<string>(AwsFileProviderConfigurationNames.AccessKeyId);
            set => _containerConfiguration.SetConfiguration(AwsFileProviderConfigurationNames.AccessKeyId, value);
        }

        public string SecretAccessKey
        {
            get => _containerConfiguration.GetConfiguration<string>(AwsFileProviderConfigurationNames.SecretAccessKey);
            set => _containerConfiguration.SetConfiguration(AwsFileProviderConfigurationNames.SecretAccessKey, value);
        }

        public bool UseCredentials
        {
            get => _containerConfiguration.GetConfigurationOrDefault(AwsFileProviderConfigurationNames.UseCredentials, false);
            set => _containerConfiguration.SetConfiguration(AwsFileProviderConfigurationNames.UseCredentials, value);
        }

        public bool UseTemporaryCredentials
        {
            get => _containerConfiguration.GetConfigurationOrDefault(AwsFileProviderConfigurationNames.UseTemporaryCredentials, false);
            set => _containerConfiguration.SetConfiguration(AwsFileProviderConfigurationNames.UseTemporaryCredentials, value);
        }

        public bool UseTemporaryFederatedCredentials
        {
            get => _containerConfiguration.GetConfigurationOrDefault(AwsFileProviderConfigurationNames.UseTemporaryFederatedCredentials, false);
            set => _containerConfiguration.SetConfiguration(AwsFileProviderConfigurationNames.UseTemporaryFederatedCredentials, value);
        }

        public string? ProfileName
        {
            get => _containerConfiguration.GetConfigurationOrDefault<string>(AwsFileProviderConfigurationNames.ProfileName);
            set => _containerConfiguration.SetConfiguration(AwsFileProviderConfigurationNames.ProfileName, value);
        }

        public string ProfilesLocation
        {
            get => _containerConfiguration.GetConfiguration<string>(AwsFileProviderConfigurationNames.ProfilesLocation);
            set => _containerConfiguration.SetConfiguration(AwsFileProviderConfigurationNames.ProfilesLocation, value);
        }

        /// <summary>
        /// Set the validity period of the temporary access credential, the unit is s, the minimum is 900, and the maximum is 129600.
        /// </summary>
        public int DurationSeconds
        {
            get => _containerConfiguration.GetConfigurationOrDefault(AwsFileProviderConfigurationNames.DurationSeconds, 0);
            set => _containerConfiguration.SetConfiguration(AwsFileProviderConfigurationNames.DurationSeconds, value);
        }

        public string Name
        {
            get => _containerConfiguration.GetConfiguration<string>(AwsFileProviderConfigurationNames.Name);
            set => _containerConfiguration.SetConfiguration(AwsFileProviderConfigurationNames.Name, value);
        }

        public string Policy
        {
            get => _containerConfiguration.GetConfiguration<string>(AwsFileProviderConfigurationNames.Policy);
            set => _containerConfiguration.SetConfiguration(AwsFileProviderConfigurationNames.Policy, value);
        }

        public string Region
        {
            get => _containerConfiguration.GetConfiguration<string>(AwsFileProviderConfigurationNames.Region);
            set => _containerConfiguration.SetConfiguration(AwsFileProviderConfigurationNames.Region, Check.NotNull(value, nameof(value)));
        }

        /// <summary>
        /// This name may only contain lowercase letters, numbers, and hyphens, and must begin with a letter or a number.
        /// Each hyphen must be preceded and followed by a non-hyphen character.
        /// The name must also be between 3 and 63 characters long.
        /// If this parameter is not specified, the ContainerName of the <see cref="FileProviderArgs"/> will be used.
        /// </summary>
        public string ContainerName
        {
            get => _containerConfiguration.GetConfiguration<string>(AwsFileProviderConfigurationNames.ContainerName);
            set => _containerConfiguration.SetConfiguration(AwsFileProviderConfigurationNames.ContainerName, value);
        }

        /// <summary>
        /// Default value: false.
        /// </summary>
        public bool CreateContainerIfNotExists
        {
            get => _containerConfiguration.GetConfigurationOrDefault(AwsFileProviderConfigurationNames.CreateContainerIfNotExists, false);
            set => _containerConfiguration.SetConfiguration(AwsFileProviderConfigurationNames.CreateContainerIfNotExists, value);
        }

        private readonly string _temporaryCredentialsCacheKey;
        public string? TemporaryCredentialsCacheKey
        {
            get => _containerConfiguration.GetConfigurationOrDefault(AwsFileProviderConfigurationNames.TemporaryCredentialsCacheKey, _temporaryCredentialsCacheKey);
            set => _containerConfiguration.SetConfiguration(AwsFileProviderConfigurationNames.TemporaryCredentialsCacheKey, value);
        }

        private readonly FileContainerConfiguration _containerConfiguration;
        public AwsFileProviderConfiguration(FileContainerConfiguration containerConfiguration)
        {
            _containerConfiguration = containerConfiguration;
            _temporaryCredentialsCacheKey = Guid.NewGuid().ToString("N");
        }
    }
}
