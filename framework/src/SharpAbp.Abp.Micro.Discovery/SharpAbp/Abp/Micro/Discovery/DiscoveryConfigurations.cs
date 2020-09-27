using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Volo.Abp;

namespace SharpAbp.Abp.Micro.Discovery
{
    public class DiscoveryConfigurations
    {
        private DiscoveryConfiguration Default => GetConfiguration<DefaultDiscovery>();

        private readonly Dictionary<string, DiscoveryConfiguration> _discoveryServices;

        public DiscoveryConfigurations()
        {
            _discoveryServices = new Dictionary<string, DiscoveryConfiguration>
            {
                //Add default service
                [ServiceNameAttribute.GetServiceName<DefaultDiscovery>()] = new DiscoveryConfiguration()
            };
        }


        public DiscoveryConfigurations Configure<T>(
          Action<DiscoveryConfiguration> configureAction)
        {
            return Configure(
                ServiceNameAttribute.GetServiceName<T>(),
                configureAction
            );
        }

        public DiscoveryConfigurations Configure(
           [NotNull] string name,
           [NotNull] Action<DiscoveryConfiguration> configureAction)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNull(configureAction, nameof(configureAction));

            configureAction(
                _discoveryServices.GetOrAdd(
                    name,
                    () => new DiscoveryConfiguration(Default)
                )
            );

            return this;
        }

        public DiscoveryConfigurations ConfigureDefault(Action<DiscoveryConfiguration> configureAction)
        {
            configureAction(Default);
            return this;
        }

        public DiscoveryConfigurations ConfigureAll(Action<string, DiscoveryConfiguration> configureAction)
        {
            foreach (var discoveryService in _discoveryServices)
            {
                configureAction(discoveryService.Key, discoveryService.Value);
            }

            return this;
        }

        [NotNull]
        public DiscoveryConfiguration GetConfiguration<T>()
        {
            return GetConfiguration(ServiceNameAttribute.GetServiceName<T>());
        }

        [NotNull]
        public DiscoveryConfiguration GetConfiguration([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            return _discoveryServices.GetOrDefault(name) ??
                   Default;
        }
    }
}
