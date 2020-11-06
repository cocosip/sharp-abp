using System;

namespace SharpAbp.Abp.FileStoring.Minio
{
    public static class MinioFileContainerConfigurationExtensions
    {
        public static MinioFileProviderConfiguration GetMinioConfiguration(
          this FileContainerConfiguration containerConfiguration)
        {
            return new MinioFileProviderConfiguration(containerConfiguration);
        }

        public static FileContainerConfiguration UseMinio(
            this FileContainerConfiguration containerConfiguration,
            Action<MinioFileProviderConfiguration> minioConfigureAction)
        {
            containerConfiguration.Provider = MinioFileProviderConfigurationNames.ProviderName;
            containerConfiguration.NamingNormalizers.TryAdd<MinioFileNamingNormalizer>();

            minioConfigureAction(new MinioFileProviderConfiguration(containerConfiguration));

            return containerConfiguration;
        }
    }
}
