using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.TenantGroupManagement.MongoDB
{
    public class MongoDbTenantGroupRepository : MongoDbRepository<ITenantGroupManagementMongoDbContext, TenantGroup, Guid>, ITenantGroupRepository
    {
        public MongoDbTenantGroupRepository(IMongoDbContextProvider<ITenantGroupManagementMongoDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public virtual async Task<TenantGroup> FindByNameAsync(
            string name,
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Name == name, includeDetails, GetCancellationToken(cancellationToken));
        }


        public virtual async Task<TenantGroup> FindExpectedByNameAsync(
            string name,
            Guid? expectedId = null,
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            return await (await GetQueryableAsync())
                .WhereIf(!name.IsNullOrWhiteSpace(), x => x.Name == name)
                .WhereIf(expectedId.HasValue, x => x.Id != expectedId.Value)
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }


        public async Task<TenantGroup> FindByTenantIdAsync(Guid? tenantId, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return await (await GetQueryableAsync())
                .WhereIf(tenantId.HasValue, x => x.Tenants.Any(x => x.TenantId == tenantId))
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<TenantGroup> FindExpectedByTenantIdAsync(Guid? tenantId, Guid? expectedId = null, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return await (await GetQueryableAsync())
                .WhereIf(tenantId.HasValue, x => x.Tenants.Any(x => x.TenantId == tenantId))
                .WhereIf(expectedId.HasValue, x => x.Id != expectedId.Value)
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<List<TenantGroup>> GetListAsync(
            string sorting = null,
            string name = "",
            bool? isActive = null,
            bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            return await (await GetQueryableAsync())
               .WhereIf(!name.IsNullOrWhiteSpace(), item => item.Name == name)
               .WhereIf(isActive.HasValue, item => item.IsActive == isActive.Value)
               .OrderBy(sorting ?? nameof(TenantGroup.Id))
               .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<List<TenantGroup>> GetPagedListAsync(
            int skipCount,
            int maxResultCount,
            string sorting = null,
            string name = "",
            bool? isActive = null,
            bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            return await (await GetQueryableAsync())
               .WhereIf(!name.IsNullOrWhiteSpace(), item => item.Name == name)
               .WhereIf(isActive.HasValue, item => item.IsActive == isActive.Value)
               .OrderBy(sorting ?? nameof(TenantGroup.Id))
               .PageBy(skipCount, maxResultCount)
               .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<int> GetCountAsync(
            string name = "",
            bool? isActive = null,
            CancellationToken cancellationToken = default)
        {
            return await (await GetQueryableAsync())
               .WhereIf(!name.IsNullOrWhiteSpace(), item => item.Name == name)
               .WhereIf(isActive.HasValue, item => item.IsActive == isActive.Value)
               .CountAsync(GetCancellationToken(cancellationToken));
        }
    }
}
