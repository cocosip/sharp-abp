using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    public interface IDatabaseConnectionCacheManager
    {
        /// <summary>
        /// Get cache by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<DatabaseConnectionCacheItem> GetAsync([NotNull] string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Remove cache by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RemoveAsync([NotNull] string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Remove many cache by names
        /// </summary>
        /// <param name="names"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RemoveManyAsync([NotNull] List<string> names, CancellationToken cancellationToken = default);
    }

}
