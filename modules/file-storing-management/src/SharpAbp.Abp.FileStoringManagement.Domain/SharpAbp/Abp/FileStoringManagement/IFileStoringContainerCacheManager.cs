using JetBrains.Annotations;
using System;
using System.Threading.Tasks;

namespace SharpAbp.Abp.FileStoringManagement
{
    public interface IFileStoringContainerCacheManager
    {
        /// <summary>
        /// Get container cache
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<FileStoringContainerCacheItem> GetCacheAsync([NotNull] string name);

        /// <summary>
        /// Update container cache
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task UpdateCacheAsync(Guid id);
    }
}
