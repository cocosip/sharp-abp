using System;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    public class DatabaseConnectionInfoCacheItem
    {
        /// <summary>
        /// DbConnection name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Database Provider
        /// </summary>
        public string DatabaseProvider { get; set; }

        /// <summary>
        /// Database connection string
        /// </summary>
        public string ConnectionString { get; set; }

        public DatabaseConnectionInfoCacheItem()
        {

        }
   
    }
}
