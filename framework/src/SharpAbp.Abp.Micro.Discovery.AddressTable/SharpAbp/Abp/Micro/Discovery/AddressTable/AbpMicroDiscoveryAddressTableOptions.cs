using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace SharpAbp.Abp.Micro.Discovery.AddressTable
{
    public class AbpMicroDiscoveryAddressTableOptions
    {
        public AddressTableConfigurations Configurations { get; }
        public AbpMicroDiscoveryAddressTableOptions()
        {
            Configurations = new AddressTableConfigurations();
        }

        public AbpMicroDiscoveryAddressTableOptions Configure(IConfiguration configuration)
        {
            var configurations = configuration.Get<List<AddressTableConfiguration>>();

            foreach (var addressTableConfiguration in configurations)
            {
                Configurations.Configure(addressTableConfiguration.Service, c =>
                {
                    foreach (var entry in addressTableConfiguration.Entries)
                    {
                        addressTableConfiguration.AddIfNotContains(entry);
                    }
                });
            }

            return this;
        }
    }
}
