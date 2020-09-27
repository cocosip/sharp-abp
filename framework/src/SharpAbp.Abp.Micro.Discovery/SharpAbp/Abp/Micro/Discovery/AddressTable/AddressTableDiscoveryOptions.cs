using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace SharpAbp.Abp.Micro.Discovery.AddressTable
{
    public class AddressTableDiscoveryOptions
    {
        private Dictionary<string, AddressTableServiceEntry> _services;

        public AddressTableDiscoveryOptions()
        {
            _services = new Dictionary<string, AddressTableServiceEntry>();

        }


    }

}
