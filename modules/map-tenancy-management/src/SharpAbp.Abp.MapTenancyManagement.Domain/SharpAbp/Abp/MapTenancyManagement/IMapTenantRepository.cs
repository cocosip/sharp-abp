using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public interface IMapTenantRepository : IBasicRepository<MapTenant, Guid>
    {
        /// <summary>
        /// Find MapTenant by code
        /// </summary>
        /// <param name="code"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MapTenant> FindByCodeAsync(string code, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Find by mapCode
        /// </summary>
        /// <param name="mapCode"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MapTenant> FindByMapCodeAsync(string mapCode, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Find by tenantId
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MapTenant> FindByTenantIdAsync(Guid tenantId, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Find expected by code
        /// </summary>
        /// <param name="code"></param>
        /// <param name="expectedId"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MapTenant> FindExpectedCodeAsync([NotNull] string code, Guid? expectedId = null, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Find expected by mapCode
        /// </summary>
        /// <param name="mapCode"></param>
        /// <param name="expectedId"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MapTenant> FindExpectedMapCodeAsync([NotNull] string mapCode, Guid? expectedId = null, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Find expected by tenantId
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="expectedId"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MapTenant> FindExpectedTenantIdAsync(Guid tenantId, Guid? expectedId = null, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get list by tenantIds
        /// </summary>
        /// <param name="tenantIds"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<MapTenant>> GetListByTenantIdsAsync(List<Guid> tenantIds, bool includeDetails = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get list by codes
        /// </summary>
        /// <param name="codes"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<MapTenant>> GetListByCodesAsync(List<string> codes, bool includeDetails = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get list by mapCodes
        /// </summary>
        /// <param name="mapCodes"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<MapTenant>> GetListByMapCodesAsync(List<string> mapCodes, bool includeDetails = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get list
        /// </summary>
        /// <param name="skipCount"></param>
        /// <param name="maxResultCount"></param>
        /// <param name="sorting"></param>
        /// <param name="tenantId"></param>
        /// <param name="tenantName"></param>
        /// <param name="code"></param>
        /// <param name="mapCode"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<MapTenant>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting = null, Guid? tenantId = null, string tenantName = "", string code = "", string mapCode = "", bool includeDetails = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get count async
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="tenantName"></param>
        /// <param name="code"></param>
        /// <param name="mapCode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> GetCountAsync(Guid? tenantId = null, string tenantName = "", string code = "", string mapCode = "", CancellationToken cancellationToken = default);
    }
}
