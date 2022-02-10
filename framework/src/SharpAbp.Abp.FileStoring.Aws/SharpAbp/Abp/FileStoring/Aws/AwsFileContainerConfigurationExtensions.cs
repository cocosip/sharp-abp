using System;

namespace SharpAbp.Abp.FileStoring.Aws
{
    public static class AwsFileContainerConfigurationExtensions
    {
        public static AwsFileProviderConfiguration GetAwsConfiguration(
           this FileContainerConfiguration containerConfiguration)
        {
            return new AwsFileProviderConfiguration(containerConfiguration);
        }

        public static FileContainerConfiguration UseAws(
           this FileContainerConfiguration containerConfiguration,
           Action<AwsFileProviderConfiguration> awsConfigureAction)
        {
            containerConfiguration.Provider = AwsFileProviderConfigurationNames.ProviderName;
            containerConfiguration.NamingNormalizers.TryAdd<AwsFileNamingNormalizer>();

            awsConfigureAction(new AwsFileProviderConfiguration(containerConfiguration));

            return containerConfiguration;
        }
    }
}
