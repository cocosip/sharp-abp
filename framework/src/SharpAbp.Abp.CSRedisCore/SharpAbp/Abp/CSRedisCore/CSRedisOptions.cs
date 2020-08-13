using System.Collections.Generic;

namespace SharpAbp.Abp.CSRedisCore
{
    public class CSRedisOptions
    {
        public List<CSRedisClientConfiguration> Configurations { get; set; }

        public CSRedisOptions()
        {
            Configurations = new List<CSRedisClientConfiguration>();
        }
    }
}
