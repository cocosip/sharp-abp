using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Collections;

namespace SharpAbp.Abp.FileStoring
{
    public class FileProviderConfiguration
    {
        public string? Provider { get; }

        public Type? LocalizationResource { get; }

        public ITypeList<IFileNamingNormalizer> DefaultNamingNormalizers { get; }

        [NotNull]
        private readonly Dictionary<string, FileProviderConfigurationItem> _items;

        public FileProviderConfiguration()
        {
            DefaultNamingNormalizers = new TypeList<IFileNamingNormalizer>();
            _items = [];
        }

        public FileProviderConfiguration([NotNull] string provider, Type localizationResource) : this()
        {
            Provider = provider;
            LocalizationResource = localizationResource;
        }

        public FileProviderConfigurationItem GetItem([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            _items.TryGetValue(name, out FileProviderConfigurationItem item);
            return item;
        }

        public FileProviderConfiguration AddItem(
            [NotNull] string name,
            [NotNull] FileProviderConfigurationItem item)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNull(item, nameof(item));
            _items.Add(name, item);
            return this;
        }

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

        public FileProviderConfiguration RemoveItem([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            _items.Remove(name);
            return this;
        }


        public Dictionary<string, FileProviderConfigurationItem> GetItems()
        {
            return _items;
        }
    }

}
