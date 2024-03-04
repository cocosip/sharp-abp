using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace SharpAbp.Abp.TransformSecurityManagement
{
    public interface ISecurityCredentialInfoRepository : IBasicRepository<SecurityCredentialInfo, Guid>
    {
        /// <summary>
        /// Find by identifier
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SecurityCredentialInfo> FindByIdentifierAsync([NotNull] string identifier, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Find expected by identifier
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="expectedId"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SecurityCredentialInfo> FindExpectedByIdentifierAsync(string identifier, Guid? expectedId = null, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get list
        /// </summary>
        /// <param name="sorting"></param>
        /// <param name="identifier"></param>
        /// <param name="keyType"></param>
        /// <param name="bizType"></param>
        /// <param name="expiresMin"></param>
        /// <param name="expiresMax"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<SecurityCredentialInfo>> GetListAsync(string sorting = null, string identifier = "", string keyType = "", string bizType = "", DateTime? expiresMin = null, DateTime? expiresMax = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get paged list
        /// </summary>
        /// <param name="skipCount"></param>
        /// <param name="maxResultCount"></param>
        /// <param name="sorting"></param>
        /// <param name="identifier"></param>
        /// <param name="keyType"></param>
        /// <param name="bizType"></param>
        /// <param name="expiresMin"></param>
        /// <param name="expiresMax"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<SecurityCredentialInfo>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting = null, string identifier = "", string keyType = "", string bizType = "", DateTime? expiresMin = null, DateTime? expiresMax = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get count
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="keyType"></param>
        /// <param name="bizType"></param>
        /// <param name="expiresMin"></param>
        /// <param name="expiresMax"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> GetCountAsync(string identifier = "", string keyType = "", string bizType = "", DateTime? expiresMin = null, DateTime? expiresMax = null, CancellationToken cancellationToken = default);
    }
}
