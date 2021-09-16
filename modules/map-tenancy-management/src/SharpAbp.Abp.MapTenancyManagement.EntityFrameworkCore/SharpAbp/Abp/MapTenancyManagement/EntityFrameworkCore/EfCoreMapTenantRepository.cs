using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.MapTenancyManagement.EntityFrameworkCore
{
    public class EfCoreMapTenantRepository : EfCoreRepository<IMapTenancyManagementDbContext, MapTenant, Guid>, IMapTenantRepository
    {
        public EfCoreMapTenantRepository(IDbContextProvider<IMapTenancyManagementDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        /// <summary>
        /// Find MapTenant by code
        /// </summary>
        /// <param name="code"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<MapTenant> FindByCodeAsync(
            [NotNull] string code,
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            return await (await GetDbSetAsync())
                .FirstOrDefaultAsync(x => x.Code == code, GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Find MapTenant by mapCode
        /// </summary>
        /// <param name="mapCode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<MapTenant> FindByMapCodeAsync(
            [NotNull] string mapCode,
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(mapCode, nameof(mapCode));
            return await (await GetDbSetAsync())
                .FirstOrDefaultAsync(x => x.MapCode == mapCode, GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Find MapTenant by tenantId
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<MapTenant> FindByTenantIdAsync(
            Guid tenantId,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .FirstOrDefaultAsync(x => x.TenantId == tenantId, GetCancellationToken(cancellationToken));
        }


        /// <summary>
        /// Find MapTenant
        /// </summary>
        /// <param name="code"></param>
        /// <param name="expectedId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<MapTenant> FindExpectedCodeAsync(
            [NotNull] string code,
            Guid? expectedId = null,
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            
            return await (await GetDbSetAsync())
                .WhereIf(!code.IsNullOrWhiteSpace(), x => x.Code == code)
                .WhereIf(expectedId.HasValue, x => x.Id != expectedId.Value)
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Find MapTenant
        /// </summary>
        /// <param name="mapCode"></param>
        /// <param name="expectedId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<MapTenant> FindExpectedMapCodeAsync(
           [NotNull] string mapCode,
           Guid? expectedId = null,
           CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(mapCode, nameof(mapCode));

            return await (await GetDbSetAsync())
                .WhereIf(!mapCode.IsNullOrWhiteSpace(), x => x.Code == mapCode)
                .WhereIf(expectedId.HasValue, x => x.Id != expectedId.Value)
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Find MapTenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="expectedId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<MapTenant> FindExpectedTenantIdAsync(
            Guid tenantId,
            Guid? expectedId = null,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .Where(x => x.TenantId == tenantId)
                .WhereIf(expectedId.HasValue, x => x.Id != expectedId.Value)
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Get list by tenant id
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<List<MapTenant>> GetListByTenantIdAsync(
            Guid tenantId,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .Where(x => x.TenantId == tenantId)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Get list by codes
        /// </summary>
        /// <param name="codes"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<List<MapTenant>> GetListByCodesAsync(
            List<string> codes,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .Where(x => codes.Contains(x.Code))
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Get list by mapCode
        /// </summary>
        /// <param name="mapCodes"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<List<MapTenant>> GetListByMapCodesAsync(
           List<string> mapCodes,
           CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .Where(x => mapCodes.Contains(x.MapCode))
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Get list
        /// </summary>
        /// <param name="skipCount"></param>
        /// <param name="maxResultCount"></param>
        /// <param name="sorting"></param>
        /// <param name="code"></param>
        /// <param name="tenantId"></param>
        /// <param name="mapCode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<List<MapTenant>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string sorting = null,
            string code = "",
            Guid? tenantId = null,
            string mapCode = "",
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!code.IsNullOrWhiteSpace(), item => item.Code == code)
                .WhereIf(tenantId.HasValue, item => item.TenantId == tenantId.Value)
                .WhereIf(!mapCode.IsNullOrWhiteSpace(), item => item.MapCode == mapCode)
                .OrderBy(sorting ?? nameof(MapTenant.Code))
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Get count async
        /// </summary>
        /// <param name="code"></param>
        /// <param name="tenantId"></param>
        /// <param name="mapCode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<int> GetCountAsync(
            string code = "",
            Guid? tenantId = null,
            string mapCode = "",
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!code.IsNullOrWhiteSpace(), item => item.Code == code)
                .WhereIf(tenantId.HasValue, item => item.TenantId == tenantId.Value)
                .WhereIf(!mapCode.IsNullOrWhiteSpace(), item => item.MapCode == mapCode)
                .CountAsync(GetCancellationToken(cancellationToken));
        }
    }
}
