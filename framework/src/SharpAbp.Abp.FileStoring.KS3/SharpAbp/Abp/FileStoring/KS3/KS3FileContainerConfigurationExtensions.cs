using System;

namespace SharpAbp.Abp.FileStoring.KS3
{
    public static class KS3FileContainerConfigurationExtensions
    {
        public static KS3FileProviderConfiguration GetKS3Configuration(
            this FileContainerConfiguration containerConfiguration)
        {
            return new KS3FileProviderConfiguration(containerConfiguration);
        }

        public static FileContainerConfiguration UseKS3(
            this FileContainerConfiguration containerConfiguration,
            Action<KS3FileProviderConfiguration> ks3ConfigureAction)
        {
            containerConfiguration.Provider = KS3FileProviderConfigurationNames.ProviderName;
            containerConfiguration.NamingNormalizers.TryAdd<KS3FileNamingNormalizer>();

            ks3ConfigureAction(new KS3FileProviderConfiguration(containerConfiguration));

            return containerConfiguration;
        }
    }
}
