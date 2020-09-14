using Volo.Abp;

namespace SharpAbp.Abp.FileStoring
{
    public class FileSystemFileProviderConfiguration
    {
        public string BasePath
        {
            get => _containerConfiguration.GetConfiguration<string>(FileSystemFileProviderConfigurationNames.BasePath);
            set => _containerConfiguration.SetConfiguration(FileSystemFileProviderConfigurationNames.BasePath, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        /// <summary>
        /// Default value: true.
        /// </summary>
        public bool AppendContainerNameToBasePath
        {
            get => _containerConfiguration.GetConfigurationOrDefault(FileSystemFileProviderConfigurationNames.AppendContainerNameToBasePath, true);
            set => _containerConfiguration.SetConfiguration(FileSystemFileProviderConfigurationNames.AppendContainerNameToBasePath, value);
        }


        /// <summary>
        /// Access server url
        /// </summary>
        public string AccessServerUrl
        {
            get => _containerConfiguration.GetConfiguration<string>(FileSystemFileProviderConfigurationNames.AccessServerUrl);
            set => _containerConfiguration.SetConfiguration(FileSystemFileProviderConfigurationNames.AccessServerUrl, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        private readonly FileContainerConfiguration _containerConfiguration;

        public FileSystemFileProviderConfiguration(FileContainerConfiguration containerConfiguration)
        {
            _containerConfiguration = containerConfiguration;
        }
    }
}
