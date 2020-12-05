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

        public ITypeList<IFileNamingNormalizer> DefaultNamingNormalizers { get; }

        [NotNull]
        private readonly Dictionary<string, FileProviderValue> _values;

        public FileProviderConfiguration()
        {
            DefaultNamingNormalizers = new TypeList<IFileNamingNormalizer>();
            _values = new Dictionary<string, FileProviderValue>();
        }

        public FileProviderConfiguration([NotNull] string provider) : this()
        {
            Provider = provider;
        }

        public FileProviderValue GetValue([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            _values.TryGetValue(name, out FileProviderValue value);

            return value;
        }


        public FileProviderConfiguration SetValue([NotNull] string name, [NotNull] FileProviderValue value)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNull(value, nameof(value));
            _values.Add(name, value);
            return this;
        }

        public FileProviderConfiguration SetValue([NotNull] string name, [NotNull] Type type, string note = "")
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            _values.Add(name, new FileProviderValue(type, note));
            return this;
        }

        public FileProviderConfiguration ClearValue([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            _values.Remove(name);
            return this;
        }

        public Dictionary<string, FileProviderValue> GetValues()
        {
            return _values;
        }
    }

}
