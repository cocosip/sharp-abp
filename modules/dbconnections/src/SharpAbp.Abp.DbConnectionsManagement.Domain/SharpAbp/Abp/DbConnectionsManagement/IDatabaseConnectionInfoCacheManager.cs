using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    public interface IDatabaseConnectionInfoCacheManager
    {
        /// <summary>
        /// Get cache by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<DatabaseConnectionInfoCacheItem> GetAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update cache
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UpdateAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Remove cache by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RemoveAsync(string name, CancellationToken cancellationToken = default);
    }

}
