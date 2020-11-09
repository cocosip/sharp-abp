using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace SharpAbp.Abp.FreeRedis
{
    public class AbpFreeRedisOptions
    {
        public FreeRedisConfigurations Clients { get; }

        public AbpFreeRedisOptions()
        {
            Clients = new FreeRedisConfigurations();
        }

        public AbpFreeRedisOptions Configure(IConfiguration configuration)
        {
            var csRedisConfigurations = configuration.Get<Dictionary<string, FreeRedisConfiguration>>();

            foreach (var kv in csRedisConfigurations)
            {
                Clients.Configure(kv.Key, c =>
                {
                    c.Mode = kv.Value.Mode;
                    c.ConnectionString = kv.Value.ConnectionString;
                    c.Sentinels = kv.Value.Sentinels;
                    c.ReadOnly = kv.Value.ReadOnly;
                });
            }
            return this;
        }
    }
}
