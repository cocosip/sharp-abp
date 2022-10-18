using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace SharpAbp.MinId
{
    public interface IMinIdTokenRepository : IBasicRepository<MinIdToken, Guid>
    {
        /// <summary>
        /// Find minIdToken by token
        /// </summary>
        /// <param name="bizType"></param>
        /// <param name="token"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MinIdToken> FindByTokenAsync([NotNull] string bizType, [NotNull] string token, CancellationToken cancellationToken = default);

        /// <summary>
        /// Find minIdToken by token
        /// </summary>
        /// <param name="bizType"></param>
        /// <param name="token"></param>
        /// <param name="expectedId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MinIdToken> FindExpectedByTokenAsync(string bizType, string token, Guid? expectedId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get list by bizType
        /// </summary>
        /// <param name="bizType"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<MinIdToken>> GetListByBizTypeAsync([NotNull] string bizType, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get list
        /// </summary>
        /// <param name="skipCount"></param>
        /// <param name="maxResultCount"></param>
        /// <param name="sorting"></param>
        /// <param name="bizType"></param>
        /// <param name="token"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<MinIdToken>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting = null, string bizType = "", string token = "", CancellationToken cancellationToken = default);

        /// <summary>
        /// Get count
        /// </summary>
        /// <param name="bizType"></param>
        /// <param name="token"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> GetCountAsync(string bizType = "", string token = "", CancellationToken cancellationToken = default);


    }
}
