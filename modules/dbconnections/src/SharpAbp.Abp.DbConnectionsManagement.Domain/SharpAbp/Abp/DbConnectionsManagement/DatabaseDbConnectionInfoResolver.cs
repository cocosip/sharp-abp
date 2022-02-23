using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.DbConnections;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    [Dependency(ServiceLifetime.Transient, ReplaceServices = true)]
    [ExposeServices(typeof(IDbConnectionInfoResolver))]
    public class DatabaseDbConnectionInfoResolver : IDbConnectionInfoResolver, ITransientDependency
    {
        protected IDatabaseConnectionInfoCacheManager ConnectionInfoCacheManager { get; }
        protected IDatabaseConnectionInfoRepository ConnectionInfoRepository { get; }

        public DatabaseDbConnectionInfoResolver(
            IDatabaseConnectionInfoCacheManager connectionInfoCacheManager,
            IDatabaseConnectionInfoRepository connectionInfoRepository)
        {
            ConnectionInfoCacheManager = connectionInfoCacheManager;
            ConnectionInfoRepository = connectionInfoRepository;
        }

        public virtual async Task<DbConnectionInfo> ResolveAsync(string dbConnectionName)
        {
            var cacheItem = await ConnectionInfoCacheManager.GetCacheAsync(dbConnectionName);
            if (cacheItem != null)
            {
                return ConvertToDbConnection(cacheItem); 
            }

            throw new AbpException($"Could not find dbConnectionInfo by dbConnectionName '{dbConnectionName}'.");
        }

        private DbConnectionInfo ConvertToDbConnection(DatabaseConnectionInfoCacheItem cacheItem)
        {
            var databaseProvider = (DatabaseProvider)Enum.Parse(typeof(DatabaseProvider), cacheItem.DatabaseProvider);
            return new DbConnectionInfo(databaseProvider, cacheItem.ConnectionString);
        }
    }
}
