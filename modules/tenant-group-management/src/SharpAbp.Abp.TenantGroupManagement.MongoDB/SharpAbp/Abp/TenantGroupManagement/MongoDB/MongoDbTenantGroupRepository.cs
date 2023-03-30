using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
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
            return await (await GetMongoQueryableAsync())
                .WhereIf(!name.IsNullOrWhiteSpace(), x => x.Name == name)
                .WhereIf(expectedId.HasValue, x => x.Id != expectedId.Value)
                .As<IMongoQueryable<TenantGroup>>()
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }


        public async Task<TenantGroup> FindByTenantIdAsync(Guid? tenantId, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync())
                .WhereIf(tenantId.HasValue, x => x.Tenants.Any(x => x.TenantId == tenantId))
                .As<IMongoQueryable<TenantGroup>>()
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<TenantGroup> FindExpectedByTenantIdAsync(Guid? tenantId, Guid? expectedId = null, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync())
                .WhereIf(tenantId.HasValue, x => x.Tenants.Any(x => x.TenantId == tenantId))
                .WhereIf(expectedId.HasValue, x => x.Id != expectedId.Value)
                .As<IMongoQueryable<TenantGroup>>()
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<List<TenantGroup>> GetListAsync(
            string sorting = null,
            string name = "",
            bool? isActive = null,
            bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync())
               .WhereIf<TenantGroup, IMongoQueryable<TenantGroup>>(!name.IsNullOrWhiteSpace(), item => item.Name == name)
               .WhereIf<TenantGroup, IMongoQueryable<TenantGroup>>(isActive.HasValue, item => item.IsActive == isActive.Value)
               .OrderBy(sorting ?? nameof(TenantGroup.Id))
               .As<IMongoQueryable<TenantGroup>>()
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
            return await (await GetMongoQueryableAsync())
               .WhereIf<TenantGroup, IMongoQueryable<TenantGroup>>(!name.IsNullOrWhiteSpace(), item => item.Name == name)
               .WhereIf<TenantGroup, IMongoQueryable<TenantGroup>>(isActive.HasValue, item => item.IsActive == isActive.Value)
               .OrderBy(sorting ?? nameof(TenantGroup.Id))
               .As<IMongoQueryable<TenantGroup>>()
               .PageBy<TenantGroup, IMongoQueryable<TenantGroup>>(skipCount, maxResultCount)
               .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<int> GetCountAsync(
            string name = "", 
            bool? isActive = null, 
            CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync())
               .WhereIf<TenantGroup, IMongoQueryable<TenantGroup>>(!name.IsNullOrWhiteSpace(), item => item.Name == name)
               .WhereIf<TenantGroup, IMongoQueryable<TenantGroup>>(isActive.HasValue, item => item.IsActive == isActive.Value)
               .As<IMongoQueryable<TenantGroup>>()
               .CountAsync(GetCancellationToken(cancellationToken));
        }


    }
}
