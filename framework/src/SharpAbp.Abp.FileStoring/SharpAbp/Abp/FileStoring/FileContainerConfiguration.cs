using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Volo.Abp;

namespace SharpAbp.Abp.FileStoring
{
    public class FileContainerConfiguration
    {
        public Type ProviderType { get; set; }

        public string ProviderName { get; set; }

        [NotNull]
        private readonly Dictionary<string, object> _properties;

        public FileContainerConfiguration()
        {
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
            return _properties.GetOrDefault(name) ?? defaultValue;
        }

        [NotNull]
        public FileContainerConfiguration SetConfiguration([NotNull] string name, [CanBeNull] object value)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNull(value, nameof(value));

            _properties[name] = value;

            return this;
        }

        [NotNull]
        public FileContainerConfiguration ClearConfiguration([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            _properties.Remove(name);

            return this;
        }
    }
}
