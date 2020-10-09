using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Volo.Abp;

namespace SharpAbp.Abp.Micro.Discovery
{
    public class DiscoveryProviderNameMappers
    {
        private readonly Dictionary<string, Type> _providers;
        public DiscoveryProviderNameMappers()
        {
            _providers = new Dictionary<string, Type>();
        }


        public DiscoveryProviderNameMappers SetProvider<T>([NotNull] string name)
        {
            return SetProvider(name, typeof(T));
        }

        public DiscoveryProviderNameMappers SetProvider([NotNull] string name, Type providerType)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            _providers[name] = providerType;
            return this;
        }

        public Type GetProviderType([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            if (!_providers.TryGetValue(name, out Type type))
            {
                return null;
            }
            return type;
        }

        public DiscoveryProviderNameMappers Remove([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            _providers.Remove(name);
            return this;
        }


    }
}
