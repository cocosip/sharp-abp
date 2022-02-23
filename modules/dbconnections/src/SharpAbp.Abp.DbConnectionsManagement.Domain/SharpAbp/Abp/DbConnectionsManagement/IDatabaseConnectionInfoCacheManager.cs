using JetBrains.Annotations;
using System;
using System.Threading.Tasks;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    public interface IDatabaseConnectionInfoCacheManager
    {
        Task<DatabaseConnectionInfoCacheItem> GetCacheAsync([NotNull] string name);
        Task UpdateCacheAsync(Guid id);
        Task RemoveCacheAsync([NotNull] string name);

    }

}
