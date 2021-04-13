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
            return await FindAsync(x => x.Code == code, true, cancellationToken);
        }

        /// <summary>
        /// Find MapTenant by code
        /// </summary>
        /// <param name="code"></param>
        /// <param name="expectedId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<MapTenant> FindExpectedAsync(
            [NotNull] string code,
            Guid? expectedId = null,
            CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync())
                .WhereIf(!code.IsNullOrWhiteSpace(), x => x.Code == code)
                .WhereIf(expectedId.HasValue, x => x.Id != expectedId.Value)
                .As<IMongoQueryable<MapTenant>>()
                .FirstOrDefaultAsync();
        }

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
        public virtual async Task<List<MapTenant>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string sorting = null,
            string code = "",
            Guid? tenantId = null,
            string mapCode = "",
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
