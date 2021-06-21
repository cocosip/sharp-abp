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
            foreach (var mapTenancyKv in mapTenancyConfigurations)
            {
                Mappers.Configure(mapTenancyKv.Key, c =>
                {
                    c.TenantId = mapTenancyKv.Value.TenantId;
                    c.Code = mapTenancyKv.Value.Code;
                    c.MapCode = mapTenancyKv.Value.MapCode;
                });
            }
            return this;
        }
    }
}
