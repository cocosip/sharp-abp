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


namespace SharpAbp.Abp.FileStoringManagement.EntityFrameworkCore
{
    public class EfCoreFileStoringContainerRepository : EfCoreRepository<IFileStoringManagementDbContext, FileStoringContainer, Guid>, IFileStoringContainerRepository
    {
        public EfCoreFileStoringContainerRepository(IDbContextProvider<IFileStoringManagementDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        /// <summary>
        /// Find container by name
        /// </summary>
        /// <param name="name">container name</param>
        /// <param name="includeDetails">include details</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<FileStoringContainer> FindByNameAsync(
            [NotNull] string name,
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .FirstOrDefaultAsync(x => x.Name == name, GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Find container by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="expectedId"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<FileStoringContainer> FindExpectedByNameAsync(
            string name,
            Guid? expectedId = null,
            bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .WhereIf(!name.IsNullOrWhiteSpace(), x => x.Name == name)
                .WhereIf(expectedId.HasValue, x => x.Id != expectedId.Value)
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Get List
        /// </summary>
        /// <param name="skipCount"></param>
        /// <param name="maxResultCount"></param>
        /// <param name="sorting"></param>
        /// <param name="includeDetails"></param>
        /// <param name="name"></param>
        /// <param name="provider"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<List<FileStoringContainer>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string sorting = null,
            bool includeDetails = true,
            string name = "",
            string provider = "",
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .WhereIf(!name.IsNullOrWhiteSpace(), item => item.Name == name)
                .WhereIf(!provider.IsNullOrWhiteSpace(), item => item.Provider == provider)
                .OrderBy(sorting ?? nameof(FileStoringContainer.Name))
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Get List
        /// </summary>
        /// <param name="sorting"></param>
        /// <param name="includeDetails"></param>
        /// <param name="name"></param>
        /// <param name="provider"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<List<FileStoringContainer>> GetListAsync(
            string sorting = null,
            bool includeDetails = true,
            string name = "",
            string provider = "",
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .WhereIf(!name.IsNullOrWhiteSpace(), item => item.Name == name)
                .WhereIf(!provider.IsNullOrWhiteSpace(), item => item.Provider == provider)
                .OrderBy(sorting ?? nameof(FileStoringContainer.Name))
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Get count async
        /// </summary>
        /// <param name="name"></param>
        /// <param name="provider"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<int> GetCountAsync(
            string name = "",
            string provider = "",
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                  .WhereIf(!name.IsNullOrWhiteSpace(), item => item.Name == name)
                  .WhereIf(!provider.IsNullOrWhiteSpace(), item => item.Provider == provider)
                  .CountAsync(GetCancellationToken(cancellationToken));
        }

        public override async Task<IQueryable<FileStoringContainer>> WithDetailsAsync()
        {
            return (await GetQueryableAsync()).IncludeDetails();
        }

    }
}
