using System;

namespace SharpAbp.Abp.FileStoring.FastDFS
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
            containerConfiguration.Provider = FastDFSFileProviderConfigurationNames.ProviderName;
            containerConfiguration.NamingNormalizers.TryAdd<FastDFSFileNamingNormalizer>();

            fastDFSConfigureAction(new FastDFSFileProviderConfiguration(containerConfiguration));

            return containerConfiguration;
        }
    }
}
