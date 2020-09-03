using System;

namespace SharpAbp.Abp.FileStoring
{
    public static class FileSystemFileContainerConfigurationExtensions
    {
        public static FileSystemFileProviderConfiguration GetFileSystemConfiguration(
            this FileContainerConfiguration containerConfiguration)
        {
            return new FileSystemFileProviderConfiguration(containerConfiguration);
        }

        public static FileContainerConfiguration UseFileSystem(
            this FileContainerConfiguration containerConfiguration,
            Action<FileSystemFileProviderConfiguration> fileSystemConfigureAction)
        {
            containerConfiguration.ProviderType = typeof(FileSystemFileProvider);
            containerConfiguration.NamingNormalizers.TryAdd<FileSystemFileNamingNormalizer>();

            fileSystemConfigureAction(new FileSystemFileProviderConfiguration(containerConfiguration));

            return containerConfiguration;
        }
    }
}
