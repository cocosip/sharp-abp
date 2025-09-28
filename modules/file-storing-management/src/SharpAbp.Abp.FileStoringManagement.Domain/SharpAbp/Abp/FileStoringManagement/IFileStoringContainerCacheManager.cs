using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.FileStoringManagement
{
    /// <summary>
    /// Defines the contract for managing file storing container cache operations.
    /// This service provides high-performance caching capabilities for file storage containers
    /// to reduce database queries and improve application response times. It supports
    /// tenant-aware caching with automatic cache key generation and bulk operations.
    /// </summary>
    public interface IFileStoringContainerCacheManager
    {
        /// <summary>
        /// Retrieves a file storing container from cache or loads it from the database if not cached.
        /// This method implements a cache-aside pattern, automatically loading container data
        /// from the repository and storing it in cache for subsequent requests. The cache key
        /// is calculated based on the current tenant context and container name.
        /// </summary>
        /// <param name="name">The unique name of the container to retrieve.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains
        /// the cached container item, or null if the container does not exist.
        /// </returns>
        Task<FileStoringContainerCacheItem> GetAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes a specific file storing container from cache by name.
        /// This method is typically called when a container is updated or deleted
        /// to ensure cache consistency. The cache key is calculated based on the
        /// current tenant context and container name.
        /// </summary>
        /// <param name="name">The unique name of the container to remove from cache.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous cache removal operation.</returns>
        Task RemoveAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes multiple file storing containers from cache in a single operation.
        /// This method provides efficient bulk cache invalidation for scenarios where
        /// multiple containers need to be evicted simultaneously. It filters out null
        /// or whitespace names and calculates cache keys based on the current tenant context.
        /// </summary>
        /// <param name="names">The list of container names to remove from cache.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous bulk cache removal operation.</returns>
        Task RemoveManyAsync(List<string> names, CancellationToken cancellationToken = default);
    }
}
