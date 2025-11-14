using JetBrains.Annotations;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.MapTenancyManagement.MongoDB
{
    public class MongoDbMapTenantRepository : MongoDbRepository<IMapTenancyManagementMongoDbContext, MapTenant, Guid>, IMapTenantRepository
    {
        public MongoDbMapTenantRepository(IMongoDbContextProvider<IMapTenancyManagementMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        /// <summary>
        /// Find by code
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
            return await FindAsync(x => x.Code == code, includeDetails, GetCancellationToken(cancellationToken));
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
            return await FindAsync(x => x.MapCode == mapCode, includeDetails, GetCancellationToken(cancellationToken));
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
            return await FindAsync(x => x.TenantId == tenantId, includeDetails, GetCancellationToken(cancellationToken));
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
            return await (await GetQueryableAsync())
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
            return await (await GetQueryableAsync())
                .WhereIf(!mapCode.IsNullOrWhiteSpace(), x => x.MapCode == mapCode)
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
            return await (await GetQueryableAsync())
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
            return await (await GetQueryableAsync())
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
        public async Task<List<MapTenant>> GetListByCodesAsync(
            List<string> codes,
            bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            return await (await GetQueryableAsync())
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
        public async Task<List<MapTenant>> GetListByMapCodesAsync(
            List<string> mapCodes,
            bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            return await (await GetQueryableAsync())
                .Where(x => mapCodes.Contains(x.MapCode))
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Retrieves a list of map tenants by multiple tenant name identifiers using MongoDB LINQ
        /// </summary>
        /// <param name="tenantNames">The collection of tenant names to search for</param>
        /// <param name="includeDetails">Whether to include related entity details in the results; defaults to false</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation; defaults to CancellationToken.None</param>
        /// <returns>A list of map tenants with the specified tenant names</returns>
        public virtual async Task<List<MapTenant>> GetListByTenantNamesAsync(
            List<string> tenantNames,
            bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            return await (await GetQueryableAsync())
                .Where(x => tenantNames.Contains(x.TenantName))
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Get paged list
        /// </summary>
        /// <param name="skipCount"></param>
        /// <param name="maxResultCount"></param>
        /// <param name="sorting"></param>
        /// <param name="filter"></param>
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
            string filter = "",
            Guid? tenantId = null,
            string tenantName = "",
            string code = "",
            string mapCode = "",
            bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            return await (await GetQueryableAsync())
                .WhereIf(!filter.IsNullOrWhiteSpace(), item => item.TenantName.Contains(tenantName) || item.Code.Contains(filter))
                .WhereIf(tenantId.HasValue, item => item.TenantId == tenantId.Value)
                .WhereIf(!tenantName.IsNullOrWhiteSpace(), item => item.TenantName.Contains(tenantName))
                .WhereIf(!code.IsNullOrWhiteSpace(), item => item.Code.Contains(code))
                .WhereIf(!mapCode.IsNullOrWhiteSpace(), item => item.MapCode.Contains(code))
                .OrderBy(sorting ?? nameof(MapTenant.Code))
                .PageBy(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        ///  Get count async
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="tenantId"></param>
        /// <param name="tenantName"></param>
        /// <param name="code"></param>
        /// <param name="mapCode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<int> GetCountAsync(
            string filter = "",
            Guid? tenantId = null,
            string tenantName = "",
            string code = "",
            string mapCode = "",
            CancellationToken cancellationToken = default)
        {
            return await (await GetQueryableAsync())
                .WhereIf(!filter.IsNullOrWhiteSpace(), item => item.TenantName.Contains(tenantName) || item.Code.Contains(filter))
                .WhereIf(tenantId.HasValue, item => item.TenantId == tenantId.Value)
                .WhereIf(!tenantName.IsNullOrWhiteSpace(), item => item.TenantName.Contains(tenantName))
                .WhereIf(!code.IsNullOrWhiteSpace(), item => item.Code.Contains(code))
                .WhereIf(!mapCode.IsNullOrWhiteSpace(), item => item.MapCode.Contains(mapCode))
                .CountAsync(GetCancellationToken(cancellationToken));
        }


    }
}
