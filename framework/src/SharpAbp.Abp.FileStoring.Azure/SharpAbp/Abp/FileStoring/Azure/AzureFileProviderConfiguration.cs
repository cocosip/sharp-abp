using Volo.Abp;

namespace SharpAbp.Abp.FileStoring.Azure
{
    public class AzureFileProviderConfiguration
    {
        public string ConnectionString
        {
            get => _containerConfiguration.GetConfiguration<string>(AzureFileProviderConfigurationNames.ConnectionString);
            set => _containerConfiguration.SetConfiguration(AzureFileProviderConfigurationNames.ConnectionString, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        /// <summary>
        /// This name may only contain lowercase letters, numbers, and hyphens, and must begin with a letter or a number.
        /// Each hyphen must be preceded and followed by a non-hyphen character.
        /// The name must also be between 3 and 63 characters long.
        /// If this parameter is not specified, the ContainerName of the <see cref="FileProviderArgs"/> will be used.
        /// </summary>
        public string ContainerName
        {
            get => _containerConfiguration.GetConfiguration<string>(AzureFileProviderConfigurationNames.ContainerName);
            set => _containerConfiguration.SetConfiguration(AzureFileProviderConfigurationNames.ContainerName, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        /// <summary>
        /// Default value: false.
        /// </summary>
        public bool CreateContainerIfNotExists
        {
            get => _containerConfiguration.GetConfigurationOrDefault(AzureFileProviderConfigurationNames.CreateContainerIfNotExists, false);
            set => _containerConfiguration.SetConfiguration(AzureFileProviderConfigurationNames.CreateContainerIfNotExists, value);
        }

        private readonly FileContainerConfiguration _containerConfiguration;

        public AzureFileProviderConfiguration(FileContainerConfiguration containerConfiguration)
        {
            _containerConfiguration = containerConfiguration;
        }
    }
}
