using System.Collections.Generic;
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
        /// Remove container cache by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RemoveAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Remove many container cache
        /// </summary>
        /// <param name="names"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RemoveManyAsync(List<string> names, CancellationToken cancellationToken = default);
    }
}
