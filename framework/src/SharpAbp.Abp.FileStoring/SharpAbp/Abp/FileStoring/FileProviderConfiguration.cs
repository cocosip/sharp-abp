using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Collections;

namespace SharpAbp.Abp.FileStoring
{
    public class FileProviderConfiguration
    {
        public string Provider { get; }

        public Type LocalizationResource { get; }

        public ITypeList<IFileNamingNormalizer> DefaultNamingNormalizers { get; }

        [NotNull]
        private readonly Dictionary<string, Type> _valueTypes;

        public FileProviderConfiguration()
        {
            DefaultNamingNormalizers = new TypeList<IFileNamingNormalizer>();
            _valueTypes = new Dictionary<string, Type>();
        }

        public FileProviderConfiguration([NotNull] string provider, Type localizationResource) : this()
        {
            Provider = provider;
            LocalizationResource = localizationResource;
        }

        public Type GetValueType([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            _valueTypes.TryGetValue(name, out Type type);
            return type;
        }

        public FileProviderConfiguration SetValueType([NotNull] string name, [NotNull] Type type)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNull(type, nameof(type));
            _valueTypes.Add(name, type);
            return this;
        }

        public FileProviderConfiguration ClearValueType([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            _valueTypes.Remove(name);
            return this;
        }

        public Dictionary<string, Type> GetValueTypes()
        {
            return _valueTypes;
        }
    }

}
