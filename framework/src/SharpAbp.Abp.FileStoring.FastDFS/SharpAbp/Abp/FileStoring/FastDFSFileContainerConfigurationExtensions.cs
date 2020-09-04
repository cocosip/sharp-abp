using System;

namespace SharpAbp.Abp.FileStoring
{
    public static class FastDFSFileContainerConfigurationExtensions
    {
        public static FastDFSFileProviderConfiguration GetFastDFSConfiguration(
        this FileContainerConfiguration containerConfiguration)
        {
            return new FastDFSFileProviderConfiguration(containerConfiguration);
        }

        public static FileContainerConfiguration UseFastDFS(
            this FileContainerConfiguration containerConfiguration,
            Action<FastDFSFileProviderConfiguration> fastDFSConfigureAction)
        {
            containerConfiguration.ProviderType = typeof(FastDFSFileProvider);

            fastDFSConfigureAction(new FastDFSFileProviderConfiguration(containerConfiguration));

            return containerConfiguration;
        }
    }
}
