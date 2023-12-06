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
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<MapTenant> FindByCodeAsync(
            [NotNull] string code,
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .FirstOrDefaultAsync(x => x.Code == code, GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Find by mapCode
        /// </summary>
        /// <param name="mapCode"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<MapTenant> FindByMapCodeAsync(
            [NotNull] string mapCode,
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(mapCode, nameof(mapCode));
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .FirstOrDefaultAsync(x => x.MapCode == mapCode, GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Find by tenantId
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<MapTenant> FindByTenantIdAsync(
            Guid tenantId,
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .FirstOrDefaultAsync(x => x.TenantId == tenantId, GetCancellationToken(cancellationToken));
        }


        /// <summary>
        /// Find expected by code
        /// </summary>
        /// <param name="code"></param>
        /// <param name="expectedId"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<MapTenant> FindExpectedCodeAsync(
            [NotNull] string code,
            Guid? expectedId = null,
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));

            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .WhereIf(!code.IsNullOrWhiteSpace(), x => x.Code == code)
                .WhereIf(expectedId.HasValue, x => x.Id != expectedId.Value)
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Find expected by mapCode
        /// </summary>
        /// <param name="mapCode"></param>
        /// <param name="expectedId"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<MapTenant> FindExpectedMapCodeAsync(
           [NotNull] string mapCode,
           Guid? expectedId = null,
           bool includeDetails = true,
           CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(mapCode, nameof(mapCode));

            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .WhereIf(!mapCode.IsNullOrWhiteSpace(), x => x.Code == mapCode)
                .WhereIf(expectedId.HasValue, x => x.Id != expectedId.Value)
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Find expected by tenantId
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="expectedId"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<MapTenant> FindExpectedTenantIdAsync(
            Guid tenantId,
            Guid? expectedId = null,
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .Where(x => x.TenantId == tenantId)
                .WhereIf(expectedId.HasValue, x => x.Id != expectedId.Value)
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Get list by tenantIds
        /// </summary>
        /// <param name="tenantIds"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<List<MapTenant>> GetListByTenantIdsAsync(
            List<Guid> tenantIds,
            bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .Where(x => tenantIds.Contains(x.TenantId))
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Get list by codes
        /// </summary>
        /// <param name="codes"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<List<MapTenant>> GetListByCodesAsync(
            List<string> codes,
            bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .Where(x => codes.Contains(x.Code))
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Get list by mapCodes
        /// </summary>
        /// <param name="mapCodes"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<List<MapTenant>> GetListByMapCodesAsync(
           List<string> mapCodes,
           bool includeDetails = false,
           CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .Where(x => mapCodes.Contains(x.MapCode))
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Get paged list
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
        public virtual async Task<List<MapTenant>> GetPagedListAsync(
            int skipCount,
            int maxResultCount,
            string sorting = null,
            Guid? tenantId = null,
            string tenantName = "",
            string code = "",
            string mapCode = "",
            bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .WhereIf(tenantId.HasValue, item => item.TenantId == tenantId.Value)
                .WhereIf(!tenantName.IsNullOrWhiteSpace(), item => item.TenantName.Contains(tenantName))
                .WhereIf(!code.IsNullOrWhiteSpace(), item => item.Code.Contains(code))
                .WhereIf(!mapCode.IsNullOrWhiteSpace(), item => item.MapCode.Contains(mapCode))
                .OrderBy(sorting ?? nameof(MapTenant.Code))
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Get count async
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="tenantName"></param>
        /// <param name="code"></param>
        /// <param name="mapCode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<int> GetCountAsync(
            Guid? tenantId = null,
            string tenantName = "",
            string code = "",
            string mapCode = "",
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(tenantId.HasValue, item => item.TenantId == tenantId.Value)
                .WhereIf(!tenantName.IsNullOrWhiteSpace(), item => item.TenantName.Contains(tenantName))
                .WhereIf(!code.IsNullOrWhiteSpace(), item => item.Code.Contains(code))
                .WhereIf(!mapCode.IsNullOrWhiteSpace(), item => item.MapCode.Contains(code))
                .CountAsync(GetCancellationToken(cancellationToken));
        }
    }
}
