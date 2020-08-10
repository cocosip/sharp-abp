using System.Collections.Generic;

namespace SharpAbp.Abp.CSRedisCore
{
    public class CSRedisOption
    {
        public List<CSRedisClientConfiguration> Configurations { get; set; }

        public CSRedisOption()
        {
            Configurations = new List<CSRedisClientConfiguration>();
        }
    }
}
