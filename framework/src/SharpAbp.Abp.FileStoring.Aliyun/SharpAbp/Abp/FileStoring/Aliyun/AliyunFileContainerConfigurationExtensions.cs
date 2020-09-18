using System;

namespace SharpAbp.Abp.FileStoring.Aliyun
{
    public static class AliyunFileContainerConfigurationExtensions
    {
        public static AliyunFileProviderConfiguration GetAliyunConfiguration(
            this FileContainerConfiguration containerConfiguration)
        {
            return new AliyunFileProviderConfiguration(containerConfiguration);
        }

        public static FileContainerConfiguration UseAliyun(
            this FileContainerConfiguration containerConfiguration,
            Action<AliyunFileProviderConfiguration> aliyunConfigureAction)
        {
            containerConfiguration.ProviderType = typeof(AliyunFileProvider);
            containerConfiguration.NamingNormalizers.TryAdd<AliyunFileNamingNormalizer>();

            aliyunConfigureAction(new AliyunFileProviderConfiguration(containerConfiguration));

            return containerConfiguration;
        }
    }
}
