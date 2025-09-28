using JetBrains.Annotations;
using Volo.Abp;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    /// <summary>
    /// Extension methods for <see cref="DatabaseConnectionInfo"/> class
    /// </summary>
    public static class DatabaseConnectionInfoExtensions
    {
        /// <summary>
        /// Converts a <see cref="DatabaseConnectionInfo"/> instance to a <see cref="DatabaseConnectionCacheItem"/> for caching purposes
        /// </summary>
        /// <param name="databaseConnectionInfo">The database connection information to convert</param>
        /// <returns>A <see cref="DatabaseConnectionCacheItem"/> instance containing the connection details, or null if the input is null</returns>
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
