using Microsoft.EntityFrameworkCore;
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
    /// <summary>
    /// Entity Framework Core implementation of the tenant group repository.
    /// Provides data access operations for tenant groups using Entity Framework Core.
    /// </summary>
    public class EfCoreTenantGroupRepository : EfCoreRepository<ITenantGroupManagementDbContext, TenantGroup, Guid>, ITenantGroupRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EfCoreTenantGroupRepository"/> class.
        /// </summary>
        /// <param name="dbContextProvider">The database context provider.</param>
        public EfCoreTenantGroupRepository(IDbContextProvider<ITenantGroupManagementDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        /// <summary>
        /// Finds a tenant group by its normalized name.
        /// </summary>
        /// <param name="normalizedName">The normalized name of the tenant group to find.</param>
        /// <param name="includeDetails">A value indicating whether to include related entities in the result.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>The tenant group with the specified normalized name, or null if not found.</returns>
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
        /// Finds a tenant group by its normalized name, excluding the specified expected ID.
        /// This method is typically used for uniqueness validation during updates.
        /// </summary>
        /// <param name="normalizedName">The normalized name of the tenant group to find.</param>
        /// <param name="expectedId">The ID to exclude from the search (typically the current entity's ID being updated).</param>
        /// <param name="includeDetails">A value indicating whether to include related entities in the result.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>The tenant group with the specified normalized name excluding the expected ID, or null if not found.</returns>
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
        /// Finds a tenant group that contains the specified tenant ID.
        /// </summary>
        /// <param name="tenantId">The ID of the tenant to search for within tenant groups.</param>
        /// <param name="includeDetails">A value indicating whether to include related entities in the result.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>The tenant group containing the specified tenant ID, or null if not found.</returns>
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
        /// Finds a tenant group that contains the specified tenant ID, excluding the specified expected ID.
        /// This method is typically used for uniqueness validation during updates.
        /// </summary>
        /// <param name="tenantId">The ID of the tenant to search for within tenant groups.</param>
        /// <param name="expectedId">The ID to exclude from the search (typically the current entity's ID being updated).</param>
        /// <param name="includeDetails">A value indicating whether to include related entities in the result.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>The tenant group containing the specified tenant ID excluding the expected ID, or null if not found.</returns>
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
        /// Gets a list of tenant groups based on the specified filtering and sorting criteria.
        /// </summary>
        /// <param name="sorting">The sorting expression. If null, defaults to ordering by ID.</param>
        /// <param name="name">The name filter to search for tenant groups containing this value.</param>
        /// <param name="isActive">The active status filter. If null, includes both active and inactive tenant groups.</param>
        /// <param name="includeDetails">A value indicating whether to include related entities in the result.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>A list of tenant groups matching the specified criteria.</returns>
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
        /// Gets a paged list of tenant groups based on the specified filtering, sorting, and pagination criteria.
        /// </summary>
        /// <param name="skipCount">The number of records to skip for pagination.</param>
        /// <param name="maxResultCount">The maximum number of records to return.</param>
        /// <param name="sorting">The sorting expression. If null, defaults to ordering by ID.</param>
        /// <param name="name">The name filter to search for tenant groups containing this value.</param>
        /// <param name="isActive">The active status filter. If null, includes both active and inactive tenant groups.</param>
        /// <param name="includeDetails">A value indicating whether to include related entities in the result.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>A paged list of tenant groups matching the specified criteria.</returns>
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
        /// Gets the total count of tenant groups based on the specified filtering criteria.
        /// </summary>
        /// <param name="name">The name filter to search for tenant groups containing this value.</param>
        /// <param name="isActive">The active status filter. If null, includes both active and inactive tenant groups.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>The total count of tenant groups matching the specified criteria.</returns>
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

        /// <summary>
        /// Gets a queryable with all related entities included.
        /// </summary>
        /// <returns>A queryable with all related entities included.</returns>
        public override async Task<IQueryable<TenantGroup>> WithDetailsAsync()
        {
            return (await GetQueryableAsync()).IncludeDetails();
        }
    }
}
