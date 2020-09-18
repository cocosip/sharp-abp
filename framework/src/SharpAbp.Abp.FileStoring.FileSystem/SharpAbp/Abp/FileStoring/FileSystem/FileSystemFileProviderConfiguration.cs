using Volo.Abp;

namespace SharpAbp.Abp.FileStoring.FileSystem
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
        public string HttpServer
        {
            get => _containerConfiguration.GetConfiguration<string>(FileSystemFileProviderConfigurationNames.HttpServer);
            set => _containerConfiguration.SetConfiguration(FileSystemFileProviderConfigurationNames.HttpServer, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        private readonly FileContainerConfiguration _containerConfiguration;

        public FileSystemFileProviderConfiguration(FileContainerConfiguration containerConfiguration)
        {
            _containerConfiguration = containerConfiguration;
        }
    }
}
