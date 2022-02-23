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

        /// <summary>
        /// Remove cache
        /// </summary>
        /// <param name="code"></param>
        /// <param name="mapCode"></param>
        /// <returns></returns>
        Task RemoveCacheAsync([NotNull] string code, [NotNull] string mapCode);

        /// <summary>
        /// Get all cache
        /// </summary>
        /// <returns></returns>
        Task<AllMapTenantCacheItem> GetAllCacheAsync();

        /// <summary>
        /// Update all cache
        /// </summary>
        /// <returns></returns>
        Task UpdateAllCacheAsync();
    }
}