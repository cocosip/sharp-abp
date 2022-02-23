using System.Collections.Generic;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    public class AllDatabaseConnectionInfoCacheItem
    {
        public List<DatabaseConnectionInfoCacheItem> DatabaseConnectionInfos { get; set; }
        public AllDatabaseConnectionInfoCacheItem()
        {
            DatabaseConnectionInfos = new List<DatabaseConnectionInfoCacheItem>();
        }
    }
}
