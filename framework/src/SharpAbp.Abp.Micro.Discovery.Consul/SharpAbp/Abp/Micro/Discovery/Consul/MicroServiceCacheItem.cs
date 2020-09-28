using System.Collections.Generic;

namespace SharpAbp.Abp.Micro.Discovery.Consul
{
    public class MicroServiceCacheItem
    {
        public string Name { get; set; }

        public List<MicroService> Services { get; set; }

        public MicroServiceCacheItem()
        {
            Services = new List<MicroService>();
        }
    }
}
