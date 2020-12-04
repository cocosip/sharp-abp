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
        Task<FileStoringContainer> FindAsync(string name, bool includeDetails = true, CancellationToken cancellationToken = default);

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
        Task<List<FileStoringContainer>> GetListAsync(int skipCount, int maxResultCount, string sorting = null, bool includeDetails = true, string name = "", string provider = "", CancellationToken cancellationToken = default);


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
