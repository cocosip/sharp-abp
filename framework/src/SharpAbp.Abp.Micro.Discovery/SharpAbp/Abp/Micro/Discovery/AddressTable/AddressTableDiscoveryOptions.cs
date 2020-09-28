using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Volo.Abp;

namespace SharpAbp.Abp.Micro.Discovery.AddressTable
{
    public class AddressTableDiscoveryOptions
    {
        private readonly Dictionary<string, AddressTableService> _services;

        public AddressTableDiscoveryOptions()
        {
            _services = new Dictionary<string, AddressTableService>();
        }

        public AddressTableDiscoveryOptions Configure(List<AddressTableService> services)
        {
            foreach (var service in services)
            {
                Configure(service.Service, c =>
                {
                    c.Service = service.Service;
                    c.Entries = new List<AddressTableServiceEntry>();

                    foreach (var entry in service.Entries)
                    {
                        c.Entries.Add(new AddressTableServiceEntry()
                        {
                            Address = entry.Address,
                            Port = entry.Port,
                            Tags = entry.Tags,
                            Meta = entry.Meta
                        });
                    }
                });
            }

            return this;
        }

        public AddressTableDiscoveryOptions Configure([NotNull] string service, [NotNull] Action<AddressTableService> configureAction)
        {
            Check.NotNullOrWhiteSpace(service, nameof(service));
            Check.NotNull(configureAction, nameof(configureAction));

            configureAction(
                _services.GetOrAdd(
                    service,
                    () => new AddressTableService()
                )
            );

            return this;
        }


        public AddressTableDiscoveryOptions ConfigureAll(Action<string, AddressTableService> configureAction)
        {
            foreach (var service in _services)
            {
                configureAction(service.Key, service.Value);
            }

            return this;
        }


        public AddressTableDiscoveryOptions Remove([NotNull] string service)
        {
            Check.NotNullOrWhiteSpace(service, nameof(service));

            _services.Remove(service);

            return this;
        }


        [NotNull]
        public AddressTableService GetService([NotNull] string service)
        {
            Check.NotNullOrWhiteSpace(service, nameof(service));
            return _services.GetOrDefault(service);
        }


    }

}
