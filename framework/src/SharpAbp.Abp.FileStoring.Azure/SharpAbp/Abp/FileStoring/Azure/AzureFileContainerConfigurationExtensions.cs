using System;

namespace SharpAbp.Abp.FileStoring.Azure
{
    public static class AzureFileContainerConfigurationExtensions
    {
        public static AzureFileProviderConfiguration GetAzureConfiguration(
            this FileContainerConfiguration containerConfiguration)
        {
            return new AzureFileProviderConfiguration(containerConfiguration);
        }

        public static FileContainerConfiguration UseAzure(
            this FileContainerConfiguration containerConfiguration,
            Action<AzureFileProviderConfiguration> azureConfigureAction)
        {
            containerConfiguration.ProviderType = typeof(AzureFileProvider);
            containerConfiguration.NamingNormalizers.TryAdd<AzureFileNamingNormalizer>();

            azureConfigureAction(new AzureFileProviderConfiguration(containerConfiguration));

            return containerConfiguration;
        }
    }
}
