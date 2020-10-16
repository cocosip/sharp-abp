using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace SharpAbp.Abp.CSRedisCore
{
    public class AbpCSRedisOptions
    {
        public CSRedisConfigurations Clients { get; }

        public AbpCSRedisOptions()
        {
            Clients = new CSRedisConfigurations();
        }

        public AbpCSRedisOptions Configure(IConfiguration configuration)
        {
            var csRedisConfigurations = configuration.Get<Dictionary<string, CSRedisConfiguration>>();

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
