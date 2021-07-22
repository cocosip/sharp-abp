using JetBrains.Annotations;
using System;
using System.Threading.Tasks;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public interface IMapTenantCacheManager
    {
        /// <summary>
        /// Get cache by code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<MapTenantCacheItem> GetCacheAsync([NotNull] string code);

        /// <summary>
        /// Get mapCode cache by mapCode
        /// </summary>
        /// <param name="mapCode"></param>
        /// <returns></returns>
        Task<MapTenantMapCodeCacheItem> GetMapCodeCacheAsync([NotNull] string mapCode);

        /// <summary>
        /// Update cache by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task UpdateCacheAsync(Guid id);

    }
}