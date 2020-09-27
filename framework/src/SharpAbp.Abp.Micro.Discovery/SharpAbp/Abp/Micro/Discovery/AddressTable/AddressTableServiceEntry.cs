using System.Collections.Generic;

namespace SharpAbp.Abp.Micro.Discovery.AddressTable
{
    public class AddressTableServiceEntry
    {
        public string Service { get; set; }

        public string Address { get; set; }

        public int Port { get; set; }

        public List<string> Tags { get; set; }

        public Dictionary<string, string> Meta { get; set; }

        public AddressTableServiceEntry()
        {
            Tags = new List<string>();
            Meta = new Dictionary<string, string>();
        }
    }


}
