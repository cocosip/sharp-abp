using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.DbConnections;
using System;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    [Dependency(ServiceLifetime.Transient, ReplaceServices = true)]
    [ExposeServices(typeof(IDbConnectionInfoResolver))]
    public class DatabaseDbConnectionInfoResolver : IDbConnectionInfoResolver, ITransientDependency
    {
        protected IDistributedCache<DatabaseConnectionInfoCacheItem> ConnectionInfoCache { get; }
        protected IDatabaseConnectionInfoRepository DatabaseConnectionInfoRepository { get; }

        public DatabaseDbConnectionInfoResolver(
            IDistributedCache<DatabaseConnectionInfoCacheItem> connectionInfoCache,
            IDatabaseConnectionInfoRepository databaseConnectionInfoRepository)
        {
            ConnectionInfoCache = connectionInfoCache;
            DatabaseConnectionInfoRepository = databaseConnectionInfoRepository;
        }

        public Task<DbConnectionInfo> ResolveAsync(string dbConnectionName)
        {
            throw new NotImplementedException();
        }

        protected virtual async Task<DbConnectionInfo> GetDbConnectionInfoAsync(string name)
        {
            var cacheItem = await ConnectionInfoCache.GetOrAddAsync(
                name,
                async () =>
                {
                    var databaseConnectionInfo = await DatabaseConnectionInfoRepository.FindByNameAsync(name);
                    return databaseConnectionInfo?.AsCacheItem();
                });

            return cacheItem == null ? null : ConvertToDbConnection(cacheItem);
        }

        private DbConnectionInfo ConvertToDbConnection(DatabaseConnectionInfoCacheItem cacheItem)
        {
            var databaseProvider = (DatabaseProvider)Enum.Parse(typeof(DatabaseProvider), cacheItem.DatabaseProvider);
            return new DbConnectionInfo(databaseProvider, cacheItem.ConnectionString);
        }
    }
}
