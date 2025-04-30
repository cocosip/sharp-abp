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
        protected IDatabaseConnectionCacheManager ConnectionInfoCacheManager { get; }
        protected IDatabaseConnectionInfoRepository ConnectionInfoRepository { get; }

        public DatabaseDbConnectionInfoResolver(
            IDatabaseConnectionCacheManager connectionInfoCacheManager,
            IDatabaseConnectionInfoRepository connectionInfoRepository)
        {
            ConnectionInfoCacheManager = connectionInfoCacheManager;
            ConnectionInfoRepository = connectionInfoRepository;
        }

        public virtual async Task<DbConnectionInfo> ResolveAsync(string dbConnectionName)
        {
            var cacheItem = await ConnectionInfoCacheManager.GetAsync(dbConnectionName);
            if (cacheItem != null)
            {
                return ConvertToDbConnection(cacheItem); 
            }

            throw new AbpException($"Could not find dbConnectionInfo by dbConnectionName '{dbConnectionName}'.");
        }

        private DbConnectionInfo ConvertToDbConnection(DatabaseConnectionCacheItem cacheItem)
        {
            var databaseProvider = (DatabaseProvider)Enum.Parse(typeof(DatabaseProvider), cacheItem.DatabaseProvider);
            return new DbConnectionInfo(databaseProvider, cacheItem.ConnectionString);
        }
    }
}
