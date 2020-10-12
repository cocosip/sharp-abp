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

        public MicroService()
        {
            Tags = new List<string>();
        }
    }
}
