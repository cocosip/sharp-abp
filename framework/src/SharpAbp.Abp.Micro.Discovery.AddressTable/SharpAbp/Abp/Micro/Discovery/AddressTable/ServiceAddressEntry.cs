using System.Collections.Generic;

namespace SharpAbp.Abp.Micro.Discovery.AddressTable
{
    public class ServiceAddressEntry
    {
        public string Id { get; set; }

        public string Address { get; set; }

        public int Port { get; set; }

        public List<string> Tags { get; set; }

        public ServiceAddressEntry()
        {
            Tags = new List<string>();
        }
    }
}
