using JetBrains.Annotations;
using System.Collections.Generic;
using Volo.Abp;

namespace SharpAbp.Abp.FileStoring
{
    public class FileProviderConfigurations
    {
        private readonly Dictionary<string, FileProviderConfiguration> _providers;

        public FileProviderConfigurations()
        {
            _providers = [];
        }

        [NotNull]
        public FileProviderConfiguration GetConfiguration([NotNull] string provider)
        {
            Check.NotNullOrWhiteSpace(provider, nameof(provider));
            return _providers.GetOrDefault(provider)!;
        }

        public bool TryAdd([NotNull] FileProviderConfiguration configuration)
        {
            Check.NotNull(configuration, nameof(configuration));

            if (_providers.ContainsKey(configuration.Provider!))
            {
                return false;
            }
            _providers.Add(configuration.Provider!, configuration);
            return true;
        }


        public bool TryRemove([NotNull] string provider)
        {
            Check.NotNull(provider, nameof(provider));
            return _providers.Remove(provider);
        }

        public List<FileProviderConfiguration> GetFileProviders()
        {
            return [.. _providers.Values];
        }

    }
}
