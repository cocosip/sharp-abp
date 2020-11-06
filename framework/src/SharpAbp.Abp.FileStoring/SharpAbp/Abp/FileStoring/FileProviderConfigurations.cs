using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Volo.Abp;

namespace SharpAbp.Abp.FileStoring
{
    public class FileProviderConfigurations
    {
        private readonly Dictionary<string, FileProviderConfiguration> _providers;

        public FileProviderConfigurations()
        {
            _providers = new Dictionary<string, FileProviderConfiguration>();
        }

        public FileProviderConfiguration GetConfiguration([NotNull] Type providerType)
        {
            Check.NotNull(providerType, nameof(providerType));
            return GetConfiguration(providerType.FullName);
        }

        public FileProviderConfiguration GetConfiguration([NotNull] string provider)
        {
            Check.NotNullOrWhiteSpace(provider, nameof(provider));
            return _providers.GetOrDefault(provider);
        }

        public bool TryAdd([NotNull] FileProviderConfiguration configuration)
        {
            Check.NotNull(configuration, nameof(configuration));

            if (_providers.ContainsKey(configuration.Provider))
            {
                return false;
            }
            _providers.Add(configuration.Provider, configuration);
            return true;
        }

        public bool TryRemove([NotNull] Type providerType)
        {
            Check.NotNull(providerType, nameof(providerType));
            return TryRemove(providerType.Name);
        }

        public bool TryRemove([NotNull] string providerName)
        {
            Check.NotNull(providerName, nameof(providerName));
            return _providers.Remove(providerName);
        }

    }
}
