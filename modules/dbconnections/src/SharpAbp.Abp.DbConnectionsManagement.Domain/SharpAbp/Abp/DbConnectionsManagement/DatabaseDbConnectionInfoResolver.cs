using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.Data;
using SharpAbp.Abp.DbConnections;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    /// <summary>
    /// Database connection information resolver that resolves connection information from the database management system
    /// </summary>
    [Dependency(ServiceLifetime.Transient, ReplaceServices = true)]
    [ExposeServices(typeof(IDbConnectionInfoResolver))]
    public class DatabaseDbConnectionInfoResolver : IDbConnectionInfoResolver, ITransientDependency
    {
        /// <summary>
        /// Gets the database connection cache manager for caching connection information
        /// </summary>
        protected IDatabaseConnectionCacheManager ConnectionInfoCacheManager { get; }
        
        /// <summary>
        /// Gets the database connection information repository for data access
        /// </summary>
        protected IDatabaseConnectionInfoRepository ConnectionInfoRepository { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseDbConnectionInfoResolver"/> class
        /// </summary>
        /// <param name="connectionInfoCacheManager">The cache manager for database connection information</param>
        /// <param name="connectionInfoRepository">The repository for database connection information</param>
        public DatabaseDbConnectionInfoResolver(
            IDatabaseConnectionCacheManager connectionInfoCacheManager,
            IDatabaseConnectionInfoRepository connectionInfoRepository)
        {
            ConnectionInfoCacheManager = connectionInfoCacheManager;
            ConnectionInfoRepository = connectionInfoRepository;
        }

        /// <summary>
        /// Resolves database connection information for the specified connection name
        /// </summary>
        /// <param name="dbConnectionName">The name of the database connection to resolve</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the database connection information.</returns>
        /// <exception cref="AbpException">Thrown when no database connection configuration is found for the specified name</exception>
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
        /// Converts a database connection cache item to database connection information
        /// </summary>
        /// <param name="cacheItem">The database connection cache item to convert</param>
        /// <returns>The converted database connection information</returns>
        private DbConnectionInfo ConvertToDbConnection(DatabaseConnectionCacheItem cacheItem)
        {
            var databaseProvider = (DatabaseProvider)Enum.Parse(typeof(DatabaseProvider), cacheItem.DatabaseProvider);
            return new DbConnectionInfo(databaseProvider, cacheItem.ConnectionString);
        }
    }
}