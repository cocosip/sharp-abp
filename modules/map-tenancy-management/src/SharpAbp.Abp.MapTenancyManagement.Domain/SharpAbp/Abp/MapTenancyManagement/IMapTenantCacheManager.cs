using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public interface IMapTenantCacheManager
    {
        /// <summary>
        /// Get cache by code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<MapTenantCacheItem> GetAsync([NotNull] string code);

        /// <summary>
        /// Update cache by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task UpdateAsync(Guid id);

    }
}