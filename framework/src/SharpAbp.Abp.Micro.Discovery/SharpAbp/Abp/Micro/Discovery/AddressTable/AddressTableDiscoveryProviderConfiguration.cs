using System;
using System.Collections.Generic;
using System.Text;

namespace SharpAbp.Abp.Micro.Discovery.AddressTable
{
    public class AddressTableDiscoveryProviderConfiguration
    {

        private readonly DiscoveryConfiguration _discoveryConfiguration;

        public AddressTableDiscoveryProviderConfiguration(DiscoveryConfiguration discoveryConfiguration)
        {
            _discoveryConfiguration = discoveryConfiguration;
        }
    }
}
