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
        Task<FileStoringContainerCacheItem> GetAsync([NotNull] string name);

        /// <summary>
        /// Update container cache
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task UpdateAsync(Guid id);
    }
}
