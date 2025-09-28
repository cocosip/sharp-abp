using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    /// <summary>
    /// Defines the contract for managing database connection caching operations
    /// </summary>
    public interface IDatabaseConnectionCacheManager
    {
        /// <summary>
        /// Retrieves a database connection cache item by name
        /// </summary>
        /// <param name="name">The name of the database connection</param>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the database connection cache item</returns>
        Task<DatabaseConnectionCacheItem> GetAsync([NotNull] string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes a database connection cache item by name
        /// </summary>
        /// <param name="name">The name of the database connection to remove from cache</param>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        /// <returns>A task that represents the asynchronous remove operation</returns>
        Task RemoveAsync([NotNull] string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes multiple database connection cache items by their names
        /// </summary>
        /// <param name="names">The list of database connection names to remove from cache</param>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        /// <returns>A task that represents the asynchronous remove operation</returns>
        Task RemoveManyAsync([NotNull] List<string> names, CancellationToken cancellationToken = default);
    }

}
