using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace SharpAbp.Abp.TenantGroupManagement
{
    public interface ITenantGroupRepository : IBasicRepository<TenantGroup, Guid>
    {
        /// <summary>
        /// Find by name
        /// </summary>
        /// <param name="normalizedName"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TenantGroup> FindByNameAsync(string normalizedName, bool includeDetails = true, CancellationToken cancellationToken = default);


        /// <summary>
        /// Find expected by name
        /// </summary>
        /// <param name="normalizedName"></param>
        /// <param name="expectedId"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TenantGroup> FindExpectedByNameAsync(string normalizedName, Guid? expectedId = null, bool includeDetails = true, CancellationToken cancellationToken = default);


        /// <summary>
        /// Find by tenantId
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TenantGroup> FindByTenantIdAsync(Guid? tenantId, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Find expected by tenantId
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="expectedId"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TenantGroup> FindExpectedByTenantIdAsync(Guid? tenantId, Guid? expectedId = null, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get list
        /// </summary>
        /// <param name="sorting"></param>
        /// <param name="name"></param>
        /// <param name="isActive"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<TenantGroup>> GetListAsync(string sorting = null, string name = "", bool? isActive = null, bool includeDetails = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get paged list
        /// </summary>
        /// <param name="skipCount"></param>
        /// <param name="maxResultCount"></param>
        /// <param name="sorting"></param>
        /// <param name="name"></param>
        /// <param name="isActive"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<TenantGroup>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting = null, string name = "", bool? isActive = null, bool includeDetails = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get count
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isActive"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> GetCountAsync(string name = "", bool? isActive = null, CancellationToken cancellationToken = default);
    }
}
