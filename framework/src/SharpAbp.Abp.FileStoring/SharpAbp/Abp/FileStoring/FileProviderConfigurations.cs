using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Volo.Abp;

namespace SharpAbp.Abp.FileStoring
{
    public class FileProviderConfigurations
    {
        private readonly Dictionary<Type, FileProviderConfiguration> _providers;

        public FileProviderConfigurations()
        {
            _providers = new Dictionary<Type, FileProviderConfiguration>();
        }

        public FileProviderConfiguration GetConfiguration([NotNull] Type providerType)
        {
            Check.NotNull(providerType, nameof(providerType));
            return _providers.GetOrDefault(providerType);
        }

        public FileProviderConfiguration GetConfiguration([NotNull] string providerTypeName)
        {
            Check.NotNullOrWhiteSpace(providerTypeName, nameof(providerTypeName));
            var providerType = Type.GetType(providerTypeName);
            if (providerType != null)
            {
                return GetConfiguration(providerType);
            }

            return null;
        }

        public bool TryAdd([NotNull] FileProviderConfiguration configuration)
        {
            Check.NotNull(configuration, nameof(configuration));

            if (!_providers.ContainsKey(configuration.ProviderType))
            {
                return false;
            }
            _providers.Add(configuration.ProviderType, configuration);
            return true;
        }

        public bool TryRemove([NotNull] Type providerType)
        {
            Check.NotNull(providerType, nameof(providerType));
            return _providers.Remove(providerType);
        }

    }
}
