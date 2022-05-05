using System;

namespace SharpAbp.Abp.FileStoring.Obs
{
    public static class ObsFileContainerConfigurationExtensions
    {
        public static ObsFileProviderConfiguration GetObsConfiguration(
         this FileContainerConfiguration containerConfiguration)
        {
            return new ObsFileProviderConfiguration(containerConfiguration);
        }

        public static FileContainerConfiguration UseObs(
            this FileContainerConfiguration containerConfiguration,
            Action<ObsFileProviderConfiguration> aliyunConfigureAction)
        {
            containerConfiguration.Provider = ObsFileProviderConfigurationNames.ProviderName;
            containerConfiguration.NamingNormalizers.TryAdd<ObsFileNamingNormalizer>();

            aliyunConfigureAction(new ObsFileProviderConfiguration(containerConfiguration));

            return containerConfiguration;
        }
    }
}
