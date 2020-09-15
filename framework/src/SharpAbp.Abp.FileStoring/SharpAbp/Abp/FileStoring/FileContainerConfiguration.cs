using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Collections;

namespace SharpAbp.Abp.FileStoring
{
    public class FileContainerConfiguration
    {
        /// <summary>
        /// The provider to be used to store FILEs of this container.
        /// </summary>
        public Type ProviderType { get; set; }

        /// <summary>
        /// Indicates whether this container is multi-tenant or not.
        ///
        /// If this is <code>false</code> and your application is multi-tenant,
        /// then the container is shared by all tenants in the system.
        ///
        /// This can be <code>true</code> even if your application is not multi-tenant.
        ///
        /// Default: true.
        /// </summary>
        public bool IsMultiTenant { get; set; } = true;

        /// <summary>
        /// Whether the container support use http url to access object
        /// Default: true
        /// </summary>
        public bool HttpSupport { get; set; } = true;

        public ITypeList<IFileNamingNormalizer> NamingNormalizers { get; }

        [NotNull]
        private readonly Dictionary<string, object> _properties;

        [CanBeNull]
        private readonly FileContainerConfiguration _fallbackConfiguration;

        public FileContainerConfiguration(FileContainerConfiguration fallbackConfiguration = null)
        {
            NamingNormalizers = new TypeList<IFileNamingNormalizer>();
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
