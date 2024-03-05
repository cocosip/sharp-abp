using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace SharpAbp.Abp.CryptoVault
{
    public interface ISM2CredsRepository : IBasicRepository<SM2Creds, Guid>
    {
        /// <summary>
        /// Find by identifier
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SM2Creds> FindByIdentifierAsync([NotNull] string identifier, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Find expected by identifier
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="expectedId"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SM2Creds> FindExpectedByIdentifierAsync(string identifier, Guid? expectedId = null, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get random sm2
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="curve"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SM2Creds> GetRandomAsync(int? sourceType = null, string curve = "", bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get list
        /// </summary>
        /// <param name="sorting"></param>
        /// <param name="identifier"></param>
        /// <param name="sourceType"></param>
        /// <param name="curve"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<SM2Creds>> GetListAsync(string sorting = null, string identifier = "", int? sourceType = null, string curve = "", bool includeDetails = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get paged list
        /// </summary>
        /// <param name="skipCount"></param>
        /// <param name="maxResultCount"></param>
        /// <param name="sorting"></param>
        /// <param name="identifier"></param>
        /// <param name="sourceType"></param>
        /// <param name="curve"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<SM2Creds>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting = null, string identifier = "", int? sourceType = null, string curve = "", bool includeDetails = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get count
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="sourceType"></param>
        /// <param name="curve"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> GetCountAsync(string identifier = "", int? sourceType = null, string curve = "", CancellationToken cancellationToken = default);
    }
}
