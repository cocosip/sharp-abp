using System;
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
        Task<FileStoringContainer> FindByNameAsync(string name, bool includeDetails = true, CancellationToken cancellationToken = default);


    }
}
