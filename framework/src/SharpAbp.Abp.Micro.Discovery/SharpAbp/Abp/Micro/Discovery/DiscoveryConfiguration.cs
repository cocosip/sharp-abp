using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Volo.Abp;

namespace SharpAbp.Abp.Micro.Discovery
{
    public class DiscoveryConfiguration
    {
        /// <summary>
        /// The provider to be used to service discovery
        /// </summary>
        public Type ProviderType { get; set; }

        [NotNull]
        private readonly Dictionary<string, object> _properties;

        public DiscoveryConfiguration _fallbackConfiguration;

        public DiscoveryConfiguration(DiscoveryConfiguration fallbackConfiguration = null)
        {
            _fallbackConfiguration = fallbackConfiguration;
            _properties = new Dictionary<string, object>();
        }


        [CanBeNull]
        public T GetConfigurationOrDefault<T>(string name, T defaultValue = default)
        {
            return (T)GetConfigurationOrNull(name, defaultValue);
        }

        [CanBeNull]
        public object GetConfigurationOrNull(string name, object defaultValue = null)
        {
            return _properties.GetOrDefault(name) ??
                   _fallbackConfiguration?.GetConfigurationOrNull(name, defaultValue) ??
                   defaultValue;
        }


        [NotNull]
        public DiscoveryConfiguration SetConfiguration([NotNull] string name, [CanBeNull] object value)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNull(value, nameof(value));

            _properties[name] = value;

            return this;
        }

        [NotNull]
        public DiscoveryConfiguration ClearConfiguration([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            _properties.Remove(name);

            return this;
        }


    }
}
