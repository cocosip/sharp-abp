using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public interface IMapTenantCacheManager
    {
        /// <summary>
        /// Get cache by code
        /// </summary>
        /// <param name="code"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MapTenantCacheItem> GetAsync(string code, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get mapCode cache by mapCode
        /// </summary>
        /// <param name="mapCode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MapTenantMapCodeCacheItem> GetMapCodeAsync(string mapCode, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update cache by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UpdateAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Remove cache
        /// </summary>
        /// <param name="code"></param>
        /// <param name="mapCode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RemoveAsync(string code, string mapCode, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all cache
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<AllMapTenantCacheItem> GetAllCacheAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Update all cache
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UpdateAllCacheAsync(CancellationToken cancellationToken = default);

    }
}