using System.Collections.Generic;

namespace SharpAbp.Abp.Micro
{
    public class ServiceEntry
    {
        public string ID { get; set; }

        public string Service { get; set; }

        public string[] Tags { get; set; }

        public string Address { get; set; }

        public int Port { get; set; }

        public IDictionary<string, string> Meta { get; set; }
    }

}
