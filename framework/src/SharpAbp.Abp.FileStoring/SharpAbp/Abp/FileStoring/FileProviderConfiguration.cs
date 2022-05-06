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
        private readonly Dictionary<string, ValueTypeInfo> _valueTypes;

        public FileProviderConfiguration()
        {
            DefaultNamingNormalizers = new TypeList<IFileNamingNormalizer>();
            _valueTypes = new Dictionary<string, ValueTypeInfo>();
        }

        public FileProviderConfiguration([NotNull] string provider, Type localizationResource) : this()
        {
            Provider = provider;
            LocalizationResource = localizationResource;
        }

        public ValueTypeInfo GetValueType([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            _valueTypes.TryGetValue(name, out ValueTypeInfo info);
            return info;
        }

        public FileProviderConfiguration SetValueType([NotNull] string name, [NotNull] Type type, string eg = "")
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNull(type, nameof(type));
            _valueTypes.Add(name, new ValueTypeInfo(type, eg));
            return this;
        }

        public FileProviderConfiguration SetValueType([NotNull] string name, ValueTypeInfo info)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            _valueTypes.Add(name, info);
            return this;
        }

        public FileProviderConfiguration ClearValueType([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            _valueTypes.Remove(name);
            return this;
        }

        public Dictionary<string, ValueTypeInfo> GetValueTypes()
        {
            return _valueTypes;
        }
    }

}
