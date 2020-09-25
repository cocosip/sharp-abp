using System.Collections.Generic;

namespace SharpAbp.Abp.Micro
{
    public class MicroService
    {
        public string ID { get; set; }

        public string Service { get; set; }

        public List<string> Tags { get; set; }

        public string Address { get; set; }

        public int Port { get; set; }

        public Dictionary<string, string> Meta { get; set; }

        public MicroService()
        {
            Tags = new List<string>();
            Meta = new Dictionary<string, string>();
        }
    }
}
