using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Volo.Abp;

namespace SharpAbp.Abp.Micro.Discovery
{
    public class ServiceDiscoveryConfigurations
    {
        private ServiceDiscoveryConfiguration Default => GetConfiguration<DefaultDiscovery>();

        private readonly Dictionary<string, ServiceDiscoveryConfiguration> _discoveryServices;

        public ServiceDiscoveryConfigurations()
        {
            _discoveryServices = new Dictionary<string, ServiceDiscoveryConfiguration>
            {
                //Add default service
                [ServiceNameAttribute.GetServiceName<DefaultDiscovery>()] = new ServiceDiscoveryConfiguration()
            };
        }


        public ServiceDiscoveryConfigurations Configure<T>(
          Action<ServiceDiscoveryConfiguration> configureAction)
        {
            return Configure(
                ServiceNameAttribute.GetServiceName<T>(),
                configureAction
            );
        }

        public ServiceDiscoveryConfigurations Configure(
           [NotNull] string name,
           [NotNull] Action<ServiceDiscoveryConfiguration> configureAction)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNull(configureAction, nameof(configureAction));

            configureAction(
                _discoveryServices.GetOrAdd(
                    name,
                    () => new ServiceDiscoveryConfiguration(Default)
                )
            );

            return this;
        }

        public ServiceDiscoveryConfigurations ConfigureDefault(Action<ServiceDiscoveryConfiguration> configureAction)
        {
            configureAction(Default);
            return this;
        }

        public ServiceDiscoveryConfigurations ConfigureAll(Action<string, ServiceDiscoveryConfiguration> configureAction)
        {
            foreach (var discoveryService in _discoveryServices)
            {
                configureAction(discoveryService.Key, discoveryService.Value);
            }

            return this;
        }

        [NotNull]
        public ServiceDiscoveryConfiguration GetConfiguration<T>()
        {
            return GetConfiguration(ServiceNameAttribute.GetServiceName<T>());
        }

        [NotNull]
        public ServiceDiscoveryConfiguration GetConfiguration([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            return _discoveryServices.GetOrDefault(name) ??
                   Default;
        }
    }
}
