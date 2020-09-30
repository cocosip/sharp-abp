using System.Collections.Generic;

namespace SharpAbp.Abp.Micro
{
    public class MicroService
    {
        public string Id { get; set; }

        public string Service { get; set; }

        public string Address { get; set; }

        public int Port { get; set; }

        public List<string> Tags { get; set; }

        public Dictionary<string, string> Meta { get; set; }

        public MicroService()
        {
            Tags = new List<string>();
            Meta = new Dictionary<string, string>();
        }

        public MicroService(string id, string service, string address, int port) : this()
        {
            Id = id;
            Service = service;
            Address = address;
            Port = port;
        }
    }
}
