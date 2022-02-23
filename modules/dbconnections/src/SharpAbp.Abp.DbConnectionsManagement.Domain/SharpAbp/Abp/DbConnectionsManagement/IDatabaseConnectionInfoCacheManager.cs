using JetBrains.Annotations;
using System;
using System.Threading.Tasks;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    public interface IDatabaseConnectionInfoCacheManager
    {
        /// <summary>
        /// Get cache
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<DatabaseConnectionInfoCacheItem> GetCacheAsync([NotNull] string name);

        /// <summary>
        /// Update cahce
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task UpdateCacheAsync(Guid id);

        /// <summary>
        /// Remove cache
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task RemoveCacheAsync([NotNull] string name);

        /// <summary>
        /// Get all cache
        /// </summary>
        /// <returns></returns>
        Task<AllDatabaseConnectionInfoCacheItem> GetAllCacheAsync();

        /// <summary>
        /// Update all cache
        /// </summary>
        /// <returns></returns>
        Task UpdateAllCacheAsync();
    }

}
