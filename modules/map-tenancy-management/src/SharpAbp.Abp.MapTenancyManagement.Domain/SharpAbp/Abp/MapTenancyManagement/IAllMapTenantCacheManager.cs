using System.Threading.Tasks;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public interface IAllMapTenantCacheManager
    {
        /// <summary>
        /// Get
        /// </summary>
        /// <returns></returns>
        Task<AllMapTenantCacheItem> GetAsync();

        /// <summary>
        /// Update
        /// </summary>
        /// <returns></returns>
        Task UpdateAsync();

    }
}
