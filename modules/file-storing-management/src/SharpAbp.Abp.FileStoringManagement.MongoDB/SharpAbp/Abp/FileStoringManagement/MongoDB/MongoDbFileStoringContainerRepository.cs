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
        public async Task<FileStoringContainer> FindExpectedAsync(
            string name,
            Guid? expectedId = null,
            bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync())
                .WhereIf(!name.IsNullOrWhiteSpace(), x => x.Name == name)
                .WhereIf(expectedId.HasValue, x => x.Id != expectedId.Value)
                .As<IMongoQueryable<FileStoringContainer>>()
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
        public async Task<List<FileStoringContainer>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string sorting = "",
            bool includeDetails = true,
            string name = "",
            string provider = "",
            CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync())
                .WhereIf<FileStoringContainer, IMongoQueryable<FileStoringContainer>>(!name.IsNullOrWhiteSpace(), item => item.Name == name)
                .WhereIf<FileStoringContainer, IMongoQueryable<FileStoringContainer>>(!provider.IsNullOrWhiteSpace(), item => item.Provider == provider)
                .OrderBy(sorting ?? nameof(FileStoringContainer.Name))
                .As<IMongoQueryable<FileStoringContainer>>()
                .PageBy<FileStoringContainer, IMongoQueryable<FileStoringContainer>>(skipCount, maxResultCount)
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
        public async Task<List<FileStoringContainer>> GetListAsync(
          string sorting = null,
          bool includeDetails = true,
          string name = "",
          string provider = "",
          CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync())
               .WhereIf<FileStoringContainer, IMongoQueryable<FileStoringContainer>>(!name.IsNullOrWhiteSpace(), item => item.Name == name)
               .WhereIf<FileStoringContainer, IMongoQueryable<FileStoringContainer>>(!provider.IsNullOrWhiteSpace(), item => item.Provider == provider)
               .OrderBy(sorting ?? nameof(FileStoringContainer.Name))
               .As<IMongoQueryable<FileStoringContainer>>()
               .ToListAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Get count
        /// </summary>
        /// <param name="name"></param>
        /// <param name="provider"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<int> GetCountAsync(
            string name = "",
            string provider = "",
            CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync())
                .WhereIf<FileStoringContainer, IMongoQueryable<FileStoringContainer>>(!name.IsNullOrWhiteSpace(), item => item.Name == name)
                .WhereIf<FileStoringContainer, IMongoQueryable<FileStoringContainer>>(!provider.IsNullOrWhiteSpace(), item => item.Provider == provider)
                .As<IMongoQueryable<FileStoringContainer>>()
                .CountAsync(GetCancellationToken(cancellationToken));
        }


    }
}
