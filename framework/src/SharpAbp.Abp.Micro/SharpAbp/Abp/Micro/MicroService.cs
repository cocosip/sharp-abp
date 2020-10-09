using System.Collections.Generic;

namespace SharpAbp.Abp.Micro
{
    public class MicroService
    {
        public string Id { get; set; }

        public string Service { get; set; }

        public string Scheme { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        public string Version { get; set; }

        public List<string> Tags { get; set; }

        public MicroService()
        {
            Tags = new List<string>();
        }

        public MicroService(string id, string service, string host, int port, string scheme = "") : this()
        {
            Id = id;
            Service = service;
            Host = host;
            Port = port;
            Scheme = scheme;
        }
    }
}
