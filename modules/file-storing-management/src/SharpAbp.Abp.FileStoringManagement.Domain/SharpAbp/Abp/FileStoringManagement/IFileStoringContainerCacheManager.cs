using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.FileStoringManagement
{
    public interface IFileStoringContainerCacheManager
    {
        /// <summary>
        /// Get container cache
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<FileStoringContainerCacheItem> GetAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update container cache
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UpdateAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Remove container cache by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RemoveAsync(string name, CancellationToken cancellationToken = default);
    }
}
