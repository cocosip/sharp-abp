using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.Data;
using SharpAbp.Abp.DbConnections;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    [Dependency(ServiceLifetime.Transient, ReplaceServices = true)]
    [ExposeServices(typeof(IDbConnectionInfoResolver))]
    public class DatabaseDbConnectionInfoResolver : IDbConnectionInfoResolver, ITransientDependency
    {
        /// <summary>
        /// Database connection cache manager
        /// </summary>
        protected IDatabaseConnectionCacheManager ConnectionInfoCacheManager { get; }
        
        /// <summary>
        /// Database connection information repository
        /// </summary>
        protected IDatabaseConnectionInfoRepository ConnectionInfoRepository { get; }

        public DatabaseDbConnectionInfoResolver(
            IDatabaseConnectionCacheManager connectionInfoCacheManager,
            IDatabaseConnectionInfoRepository connectionInfoRepository)
        {
            ConnectionInfoCacheManager = connectionInfoCacheManager;
            ConnectionInfoRepository = connectionInfoRepository;
        }

        /// <summary>
        /// Resolve database connection information by name
        /// </summary>
        /// <param name="dbConnectionName">Database connection name</param>
        /// <returns>Database connection information</returns>
        public virtual async Task<DbConnectionInfo> ResolveAsync(string dbConnectionName)
        {
            var cacheItem = await ConnectionInfoCacheManager.GetAsync(dbConnectionName);
            if (cacheItem != null)
            {
                return ConvertToDbConnection(cacheItem); 
            }

            throw new AbpException($"Could not find database connection information for name '{dbConnectionName}'. Please ensure the database connection is properly configured.");
        }

        /// <summary>
        /// Convert cache item to database connection information
        /// </summary>
        /// <param name="cacheItem">Database connection cache item</param>
        /// <returns>Database connection information</returns>
        private DbConnectionInfo ConvertToDbConnection(DatabaseConnectionCacheItem cacheItem)
        {
            var databaseProvider = (DatabaseProvider)Enum.Parse(typeof(DatabaseProvider), cacheItem.DatabaseProvider);
            return new DbConnectionInfo(databaseProvider, cacheItem.ConnectionString);
        }
    }
}