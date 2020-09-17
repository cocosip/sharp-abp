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
        public Dictionary<string, object> Properties { get; private set; }

        [CanBeNull]
        public FileContainerConfiguration FallbackConfiguration { get; private set; }

        public FileContainerConfiguration(FileContainerConfiguration fallbackConfiguration = null)
        {
            NamingNormalizers = new TypeList<IFileNamingNormalizer>();
            FallbackConfiguration = fallbackConfiguration;
            Properties = new Dictionary<string, object>();
        }

        [CanBeNull]
        public T GetConfigurationOrDefault<T>(string name, T defaultValue = default)
        {
            return (T)GetConfigurationOrNull(name, defaultValue);
        }

        [CanBeNull]
        public object GetConfigurationOrNull(string name, object defaultValue = null)
        {
            return Properties.GetOrDefault(name) ??
                   FallbackConfiguration?.GetConfigurationOrNull(name, defaultValue) ??
                   defaultValue;
        }

        [NotNull]
        public FileContainerConfiguration SetConfiguration([NotNull] string name, [CanBeNull] object value)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNull(value, nameof(value));

            Properties[name] = value;

            return this;
        }

        [NotNull]
        public FileContainerConfiguration ClearConfiguration([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            Properties.Remove(name);

            return this;
        }
    }
}
