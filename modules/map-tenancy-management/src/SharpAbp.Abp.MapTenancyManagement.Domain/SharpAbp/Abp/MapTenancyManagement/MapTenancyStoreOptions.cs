using System;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class MapTenancyStoreOptions
    {
        /// <summary>
        /// Expires
        /// </summary>
        public TimeSpan StampExpiration { get; set; }

        public MapTenancyStoreOptions()
        {
            StampExpiration = TimeSpan.FromDays(1);
        }
    }
}
