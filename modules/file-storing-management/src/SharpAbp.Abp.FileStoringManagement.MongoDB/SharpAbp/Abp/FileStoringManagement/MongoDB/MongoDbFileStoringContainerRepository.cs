using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;


namespace SharpAbp.Abp.FileStoringManagement.MongoDB
{
    public class MongoDbFileStoringContainerRepository : MongoDbRepository<IFileStoringManagementMongoDbContext, FileStoringContainer, Guid>, IFileStoringContainerRepository
    {
        public MongoDbFileStoringContainerRepository(IMongoDbContextProvider<IFileStoringManagementMongoDbContext> dbContextProvider)
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
            return await FindAsync(x => x.Name == name, includeDetails, GetCancellationToken(cancellationToken));
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
            return await (await GetQueryableAsync())
                .WhereIf(!name.IsNullOrWhiteSpace(), x => x.Name == name)
                .WhereIf(expectedId.HasValue, x => x.Id != expectedId.Value)
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Get List
        /// </summary>
        /// <param name="sorting"></param>
        /// <param name="name"></param>
        /// <param name="provider"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<List<FileStoringContainer>> GetListAsync(
            string sorting = null,
            string name = "",
            string provider = "",
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            return await (await GetQueryableAsync())
               .WhereIf(!name.IsNullOrWhiteSpace(), item => item.Name == name)
               .WhereIf(!provider.IsNullOrWhiteSpace(), item => item.Provider == provider)
               .OrderBy(sorting ?? nameof(FileStoringContainer.Name))
               .ToListAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Get paged list
        /// </summary>
        /// <param name="skipCount"></param>
        /// <param name="maxResultCount"></param>
        /// <param name="sorting"></param>
        /// <param name="name"></param>
        /// <param name="provider"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<List<FileStoringContainer>> GetPagedListAsync(
            int skipCount,
            int maxResultCount,
            string sorting = "",
            string name = "",
            string provider = "",
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            return await (await GetQueryableAsync())
                .WhereIf(!name.IsNullOrWhiteSpace(), item => item.Name == name)
                .WhereIf(!provider.IsNullOrWhiteSpace(), item => item.Provider == provider)
                .OrderBy(sorting ?? nameof(FileStoringContainer.Name))
                .PageBy(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));

        }

        /// <summary>
        /// Get count
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
            return await (await GetQueryableAsync())
                .WhereIf(!name.IsNullOrWhiteSpace(), item => item.Name == name)
                .WhereIf(!provider.IsNullOrWhiteSpace(), item => item.Provider == provider)
                .CountAsync(GetCancellationToken(cancellationToken));
        }


    }
}
