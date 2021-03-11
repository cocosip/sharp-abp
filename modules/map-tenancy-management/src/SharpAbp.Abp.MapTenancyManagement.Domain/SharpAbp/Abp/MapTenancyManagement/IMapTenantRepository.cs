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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MapTenant> FindAsync([NotNull] string code, CancellationToken cancellationToken = default);

        /// <summary>
        /// Find MapTenant
        /// </summary>
        /// <param name="code"></param>
        /// <param name="exceptId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MapTenant> FindAsync([NotNull] string code, Guid? exceptId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get List
        /// </summary>
        /// <param name="skipCount"></param>
        /// <param name="maxResultCount"></param>
        /// <param name="sorting"></param>
        /// <param name="code"></param>
        /// <param name="tenantId"></param>
        /// <param name="mapCode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<MapTenant>> GetListAsync(int skipCount, int maxResultCount, string sorting = null, string code = "", Guid? tenantId = null, string mapCode = "", CancellationToken cancellationToken = default);

        /// <summary>
        /// Get count async
        /// </summary>
        /// <param name="code"></param>
        /// <param name="tenantId"></param>
        /// <param name="mapCode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> GetCountAsync(string code = "", Guid? tenantId = null, string mapCode = "", CancellationToken cancellationToken = default);
    }
}
