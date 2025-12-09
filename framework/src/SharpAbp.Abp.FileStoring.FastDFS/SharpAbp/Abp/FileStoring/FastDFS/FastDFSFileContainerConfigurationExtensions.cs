using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Volo.Abp;

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


        public static IEnumerable<string> ToTrackers([NotNull] this string value)
        {
            Check.NotNullOrWhiteSpace(value, nameof(value));

            foreach (var tracker in value.Split(','))
            {
                yield return tracker.Trim();
            }
        }

    }
}
