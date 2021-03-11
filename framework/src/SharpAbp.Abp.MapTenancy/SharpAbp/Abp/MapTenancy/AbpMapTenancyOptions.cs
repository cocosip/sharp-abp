using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace SharpAbp.Abp.MapTenancy
{
    public class AbpMapTenancyOptions
    {
        public MapTenancyConfigurations Mappers { get; }

        public AbpMapTenancyOptions()
        {
            Mappers = new MapTenancyConfigurations();
        }

        public AbpMapTenancyOptions Configure(IConfiguration configuration)
        {
            var mapTenancyConfigurations = configuration.Get<Dictionary<string, MapTenancyConfiguration>>();
            foreach (var kv in mapTenancyConfigurations)
            {
                Mappers.Configure(kv.Key, c =>
                {
                    c.TenantId = kv.Value.TenantId;
                    c.MapCode = kv.Value.MapCode;
                });
            }
            return this;
        }
    }
}
