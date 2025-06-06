﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.TenantGroupManagement.EntityFrameworkCore
{
    public class EfCoreTenantGroupRepository : EfCoreRepository<ITenantGroupManagementDbContext, TenantGroup, Guid>, ITenantGroupRepository
    {
        public EfCoreTenantGroupRepository(IDbContextProvider<ITenantGroupManagementDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        /// <summary>
        /// Find by name
        /// </summary>
        /// <param name="normalizedName"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<TenantGroup> FindByNameAsync(
            string normalizedName,
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .OrderBy(t => t.Id)
                .FirstOrDefaultAsync(t => t.NormalizedName == normalizedName, GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Find expected by name
        /// </summary>
        /// <param name="normalizedName"></param>
        /// <param name="expectedId"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<TenantGroup> FindExpectedByNameAsync(
            string normalizedName,
            Guid? expectedId = null,
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .WhereIf(!normalizedName.IsNullOrWhiteSpace(), item => item.NormalizedName == normalizedName)
                .WhereIf(expectedId.HasValue, item => item.Id != expectedId.Value)
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Find by tenantId
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<TenantGroup> FindByTenantIdAsync(
            Guid? tenantId,
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .WhereIf(tenantId.HasValue, item => item.Tenants.Any(x => x.TenantId == tenantId))
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
        public virtual async Task<TenantGroup> FindExpectedByTenantIdAsync(
            Guid? tenantId,
            Guid? expectedId = null,
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .WhereIf(tenantId.HasValue, item => item.Tenants.Any(x => x.TenantId == tenantId))
                .WhereIf(expectedId.HasValue, item => item.Id != expectedId.Value)
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        ///  Get list
        /// </summary>
        /// <param name="sorting"></param>
        /// <param name="name"></param>
        /// <param name="isActive"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<List<TenantGroup>> GetListAsync(
            string sorting = null,
            string name = "",
            bool? isActive = null,
            bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .WhereIf(!name.IsNullOrWhiteSpace(), item => item.Name.Contains(name))
                .WhereIf(isActive.HasValue, item => item.IsActive == isActive.Value)
                .OrderBy(sorting ?? nameof(TenantGroup.Id))
                .ToListAsync(GetCancellationToken(cancellationToken));
        }


        /// <summary>
        /// Get paged list
        /// </summary>
        /// <param name="skipCount"></param>
        /// <param name="maxResultCount"></param>
        /// <param name="sorting"></param>
        /// <param name="name"></param>
        /// <param name="isActive"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<List<TenantGroup>> GetPagedListAsync(
            int skipCount,
            int maxResultCount,
            string sorting = null,
            string name = "",
            bool? isActive = null,
            bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .WhereIf(!name.IsNullOrWhiteSpace(), item => item.Name.Contains(name))
                .WhereIf(isActive.HasValue, item => item.IsActive == isActive.Value)
                .OrderBy(sorting ?? nameof(TenantGroup.Id))
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Get count
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isActive"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<int> GetCountAsync(
            string name = "",
            bool? isActive = null,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!name.IsNullOrWhiteSpace(), item => item.Name.Contains(name))
                .WhereIf(isActive.HasValue, item => item.IsActive == isActive.Value)
                .CountAsync(GetCancellationToken(cancellationToken));
        }

        public override async Task<IQueryable<TenantGroup>> WithDetailsAsync()
        {
            return (await GetQueryableAsync()).IncludeDetails();
        }
    }
}
