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
        public EfCoreMapTenantRepository(IDbContextProvider<IMapTenancyManagementDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        /// <summary>
        /// Find MapTenant by code
        /// </summary>
        /// <param name="code"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<MapTenant> FindAsync(
            [NotNull] string code,
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            return await (await GetDbSetAsync())
                .FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
        }

        /// <summary>
        /// Find MapTenant
        /// </summary>
        /// <param name="code"></param>
        /// <param name="exceptId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<MapTenant> FindAsync(
            [NotNull] string code, 
            Guid? exceptId = null, 
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!code.IsNullOrWhiteSpace(), x => x.Code == code)
                .WhereIf(exceptId.HasValue, x => x.Id != exceptId.Value)
                .FirstOrDefaultAsync(cancellationToken);
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
            return await (await GetDbSetAsync())
                .WhereIf(!code.IsNullOrWhiteSpace(), item => item.Code == code)
                .WhereIf(tenantId.HasValue, item => item.TenantId == tenantId.Value)
                .WhereIf(!mapCode.IsNullOrWhiteSpace(), item => item.MapCode == mapCode)
                .OrderBy(sorting ?? nameof(MapTenant.Code))
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync(cancellationToken);
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
                .CountAsync(cancellationToken);
        }
    }
}
