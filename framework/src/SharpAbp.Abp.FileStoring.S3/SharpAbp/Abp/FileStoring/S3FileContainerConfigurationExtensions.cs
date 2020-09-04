using System;

namespace SharpAbp.Abp.FileStoring
{
    public static class S3FileContainerConfigurationExtensions
    {
        public static S3FileProviderConfiguration GetS3Configuration(
          this FileContainerConfiguration containerConfiguration)
        {
            return new S3FileProviderConfiguration(containerConfiguration);
        }

        public static FileContainerConfiguration UseS3(
            this FileContainerConfiguration containerConfiguration,
            Action<S3FileProviderConfiguration> s3ConfigureAction)
        {
            containerConfiguration.ProviderType = typeof(S3FileProvider);

            s3ConfigureAction(new S3FileProviderConfiguration(containerConfiguration));

            return containerConfiguration;
        }
    }
}
