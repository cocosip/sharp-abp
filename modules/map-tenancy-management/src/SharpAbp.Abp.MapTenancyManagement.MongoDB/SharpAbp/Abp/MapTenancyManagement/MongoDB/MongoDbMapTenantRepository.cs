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
            return await (await GetMongoQueryableAsync())
                .WhereIf(!code.IsNullOrWhiteSpace(), x => x.Code == code)
                .WhereIf(expectedId.HasValue, x => x.Id != expectedId.Value)
                .As<IMongoQueryable<MapTenant>>()
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
            return await (await GetMongoQueryableAsync())
                .WhereIf(!mapCode.IsNullOrWhiteSpace(), x => x.MapCode == mapCode)
                .WhereIf(expectedId.HasValue, x => x.Id != expectedId.Value)
                .As<IMongoQueryable<MapTenant>>()
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
            return await (await GetMongoQueryableAsync())
                .Where(x => x.TenantId == tenantId)
                .WhereIf(expectedId.HasValue, x => x.Id != expectedId.Value)
                .As<IMongoQueryable<MapTenant>>()
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
            return await (await GetMongoQueryableAsync())
                .Where(x => tenantIds.Contains(x.TenantId))
                .As<IMongoQueryable<MapTenant>>()
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
            return await (await GetMongoQueryableAsync())
                .Where(x => codes.Contains(x.Code))
                .As<IMongoQueryable<MapTenant>>()
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
            return await (await GetMongoQueryableAsync())
                .Where(x => mapCodes.Contains(x.MapCode))
                .As<IMongoQueryable<MapTenant>>()
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Get paged list
        /// </summary>
        /// <param name="skipCount"></param>
        /// <param name="maxResultCount"></param>
        /// <param name="sorting"></param>
        /// <param name="code"></param>
        /// <param name="tenantId"></param>
        /// <param name="mapCode"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<List<MapTenant>> GetPagedListAsync(
            int skipCount,
            int maxResultCount,
            string sorting = null,
            string code = "",
            Guid? tenantId = null,
            string mapCode = "",
            bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync())
               .WhereIf<MapTenant, IMongoQueryable<MapTenant>>(!code.IsNullOrWhiteSpace(), item => item.Code == code)
               .WhereIf<MapTenant, IMongoQueryable<MapTenant>>(tenantId.HasValue, item => item.TenantId == tenantId.Value)
               .WhereIf(!mapCode.IsNullOrWhiteSpace(), item => item.MapCode == mapCode)
               .OrderBy(sorting ?? nameof(MapTenant.Code))
               .As<IMongoQueryable<MapTenant>>()
               .PageBy<MapTenant, IMongoQueryable<MapTenant>>(skipCount, maxResultCount)
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
            return await (await GetMongoQueryableAsync())
               .WhereIf<MapTenant, IMongoQueryable<MapTenant>>(!code.IsNullOrWhiteSpace(), item => item.Code == code)
               .WhereIf<MapTenant, IMongoQueryable<MapTenant>>(tenantId.HasValue, item => item.TenantId == tenantId.Value)
               .WhereIf(!mapCode.IsNullOrWhiteSpace(), item => item.MapCode == mapCode)
               .As<IMongoQueryable<MapTenant>>()
               .CountAsync(GetCancellationToken(cancellationToken));
        }


    }
}
