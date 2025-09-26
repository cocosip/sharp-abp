﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Collections;

namespace SharpAbp.Abp.FileStoring
{
    /// <summary>
    /// Configuration class for file provider settings.
    /// Contains configuration items and settings for a specific file storage provider.
    /// </summary>
    public class FileProviderConfiguration
    {
        /// <summary>
        /// Gets the name of the file provider.
        /// </summary>
        public string? Provider { get; }

        /// <summary>
        /// Gets the localization resource type for this provider.
        /// </summary>
        public Type? LocalizationResource { get; }

        /// <summary>
        /// Gets the list of default naming normalizers for this provider.
        /// </summary>
        public ITypeList<IFileNamingNormalizer> DefaultNamingNormalizers { get; }

        /// <summary>
        /// Dictionary of configuration items for this provider.
        /// </summary>
        [NotNull]
        private readonly Dictionary<string, FileProviderConfigurationItem> _items;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileProviderConfiguration"/> class.
        /// </summary>
        public FileProviderConfiguration()
        {
            DefaultNamingNormalizers = new TypeList<IFileNamingNormalizer>();
            _items = [];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileProviderConfiguration"/> class with provider name and localization resource.
        /// </summary>
        /// <param name="provider">The name of the file provider</param>
        /// <param name="localizationResource">The localization resource type</param>
        public FileProviderConfiguration([NotNull] string provider, Type localizationResource) : this()
        {
            Provider = provider;
            LocalizationResource = localizationResource;
        }

        /// <summary>
        /// Gets a configuration item by its name.
        /// </summary>
        /// <param name="name">The name of the configuration item</param>
        /// <returns>The configuration item if found, otherwise null</returns>
        public FileProviderConfigurationItem GetItem([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            _items.TryGetValue(name, out FileProviderConfigurationItem item);
            return item;
        }

        /// <summary>
        /// Adds a configuration item to this provider configuration.
        /// </summary>
        /// <param name="name">The name of the configuration item</param>
        /// <param name="item">The configuration item to add</param>
        /// <returns>This configuration instance for method chaining</returns>
        public FileProviderConfiguration AddItem(
            [NotNull] string name,
            [NotNull] FileProviderConfigurationItem item)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNull(item, nameof(item));
            _items.Add(name, item);
            return this;
        }

        /// <summary>
        /// Adds a configuration item to this provider configuration with the specified parameters.
        /// </summary>
        /// <param name="name">The name of the configuration item</param>
        /// <param name="valueType">The type of the configuration value</param>
        /// <param name="eg">An example value for this configuration item</param>
        /// <param name="noteLocalizationName">The localization name for notes about this configuration item</param>
        /// <returns>This configuration instance for method chaining</returns>
        public FileProviderConfiguration AddItem(
            [NotNull] string name,
            [NotNull] Type valueType,
            string eg = "",
            string noteLocalizationName = "")
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNull(valueType, nameof(valueType));
            return AddItem(name, new FileProviderConfigurationItem(valueType, eg, noteLocalizationName));
        }

        /// <summary>
        /// Removes a configuration item from this provider configuration.
        /// </summary>
        /// <param name="name">The name of the configuration item to remove</param>
        /// <returns>This configuration instance for method chaining</returns>
        public FileProviderConfiguration RemoveItem([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            _items.Remove(name);
            return this;
        }

        /// <summary>
        /// Gets all configuration items for this provider.
        /// </summary>
        /// <returns>A dictionary of all configuration items</returns>
        public Dictionary<string, FileProviderConfigurationItem> GetItems()
        {
            return _items;
        }
    }
}