using System.Collections.Generic;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class AllMapTenantCacheItem
    {
        public List<MapTenantCacheItem> MapTenants { get; set; }

        public AllMapTenantCacheItem()
        {
            MapTenants = new List<MapTenantCacheItem>();
        }
    }
}
