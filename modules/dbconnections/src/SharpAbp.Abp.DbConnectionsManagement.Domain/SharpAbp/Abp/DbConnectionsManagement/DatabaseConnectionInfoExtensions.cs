using JetBrains.Annotations;
using Volo.Abp;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    public static class DatabaseConnectionInfoExtensions
    {
        public static DatabaseConnectionCacheItem AsCacheItem([NotNull] this DatabaseConnectionInfo databaseConnectionInfo)
        {
            Check.NotNull(databaseConnectionInfo, nameof(databaseConnectionInfo));
            if (databaseConnectionInfo != null)
            {
                var cacheItem = new DatabaseConnectionCacheItem()
                {
                    Name = databaseConnectionInfo.Name,
                    DatabaseProvider = databaseConnectionInfo.DatabaseProvider,
                    ConnectionString = databaseConnectionInfo.ConnectionString
                };

                return cacheItem;
            }
            return null;
        }
    }
}
