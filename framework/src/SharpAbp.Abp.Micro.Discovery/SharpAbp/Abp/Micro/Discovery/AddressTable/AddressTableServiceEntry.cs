using System.Collections.Generic;

namespace SharpAbp.Abp.Micro.Discovery.AddressTable
{
    public class AddressTableService
    {
        public string Service { get; set; }

        public List<AddressTableServiceEntry> Entries { get; set; }


        public AddressTableService()
        {
            Entries = new List<AddressTableServiceEntry>();
        }
    }


    public class AddressTableServiceEntry
    {
        public string Id { get; set; }

        public string Address { get; set; }

        public int Port { get; set; }

        public List<string> Tags { get; set; }

        public Dictionary<string, string> Meta { get; set; }

        public AddressTableServiceEntry()
        {
            Tags = new List<string>();
            Meta = new Dictionary<string, string>();
        }


        public AddressTableServiceEntry(string id, string address, int port) : this()
        {
            Id = id;
            Address = address;
            Port = port;
        }


        public MicroService ToMicroService(string service)
        {
            var microService = new MicroService()
            {
                Id = Id,
                Service = service,
                Address = Address,
                Port = Port,
                Tags = Tags,
                Meta = Meta
            };
            return microService;
        }

    }


}
