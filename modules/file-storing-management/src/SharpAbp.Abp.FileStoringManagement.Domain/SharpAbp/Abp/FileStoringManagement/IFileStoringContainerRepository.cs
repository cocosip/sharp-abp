using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace SharpAbp.Abp.FileStoringManagement
{
    public interface IFileStoringContainerRepository : IBasicRepository<FileStoringContainer, Guid>
    {
        /// <summary>
        /// Find container by name
        /// </summary>
        /// <param name="name">container name</param>
        /// <param name="includeDetails">include details</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<FileStoringContainer> FindByNameAsync([NotNull] string name, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Find container by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="expectedId"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<FileStoringContainer> FindExpectedByNameAsync(string name, Guid? expectedId = null, bool includeDetails = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get list
        /// </summary>
        /// <param name="sorting"></param>
        /// <param name="name"></param>
        /// <param name="provider"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<FileStoringContainer>> GetListAsync(string sorting = null, string name = "", string provider = "", bool includeDetails = false, CancellationToken cancellationToken = default);

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
        Task<List<FileStoringContainer>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting = null, string name = "", string provider = "", bool includeDetails = false, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Get count async
        /// </summary>
        /// <param name="name"></param>
        /// <param name="provider"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> GetCountAsync(string name = "", string provider = "", CancellationToken cancellationToken = default);

    }
}
