namespace SharpAbp.Abp.DbConnectionsManagement
{
    public class DatabaseConnectionCacheItem
    {
        private const string CacheKeyFormat = "n:{0}";

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


        public static string CalculateCacheKey(string name)
        {
            return string.Format(CacheKeyFormat, name);
        }
    }
}
