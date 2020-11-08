﻿using JetBrains.Annotations;
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
        private readonly Dictionary<string, Type> _properties;

        public FileProviderConfiguration([NotNull] string provider)
        {
            Provider = provider;
            DefaultNamingNormalizers = new TypeList<IFileNamingNormalizer>();
            _properties = new Dictionary<string, Type>();
        }

        public Type GetProperty([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            _properties.TryGetValue(name, out Type property);

            return property;
        }


        public FileProviderConfiguration SetProperty([NotNull] string name, [NotNull] Type property)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNull(property, nameof(property));
            _properties.Add(name, property);
            return this;
        }

        public FileProviderConfiguration ClearProperty([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            _properties.Remove(name);
            return this;
        }

        public Dictionary<string, Type> GetProperties()
        {
            return _properties;
        }
    }

}