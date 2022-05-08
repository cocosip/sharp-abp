using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace SharpAbp.MinId
{
    public interface IMinIdInfoRepository : IBasicRepository<MinIdInfo, Guid>
    {
        /// <summary>
        /// Find minIdInfo by bizType
        /// </summary>
        /// <param name="bizType"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MinIdInfo> FindByBizTypeAsync([NotNull] string bizType, CancellationToken cancellationToken = default);

        /// <summary>
        /// Find minIdInfo by bizType
        /// </summary>
        /// <param name="bizType"></param>
        /// <param name="expectedId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MinIdInfo> FindExpectedByBizTypeAsync(string bizType, Guid? expectedId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get list
        /// </summary>
        /// <param name="skipCount"></param>
        /// <param name="maxResultCount"></param>
        /// <param name="sorting"></param>
        /// <param name="bizType"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<MinIdInfo>> GetListAsync(int skipCount, int maxResultCount, string sorting = null, string bizType = "", CancellationToken cancellationToken = default);

        /// <summary>
        /// Get count
        /// </summary>
        /// <param name="bizType"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> GetCountAsync(string bizType = "", CancellationToken cancellationToken = default);
    
    }
}
