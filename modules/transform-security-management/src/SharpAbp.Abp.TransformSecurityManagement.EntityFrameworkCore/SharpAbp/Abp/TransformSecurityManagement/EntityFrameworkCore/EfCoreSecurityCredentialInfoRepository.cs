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

namespace SharpAbp.Abp.TransformSecurityManagement.EntityFrameworkCore
{
    public class EfCoreSecurityCredentialInfoRepository : EfCoreRepository<IAbpTransformSecurityManagementDbContext, SecurityCredentialInfo, Guid>, ISecurityCredentialInfoRepository
    {
        public EfCoreSecurityCredentialInfoRepository(IDbContextProvider<IAbpTransformSecurityManagementDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        /// <summary>
        /// Find by identifier
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<SecurityCredentialInfo> FindByIdentifierAsync(
            [NotNull] string identifier,
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(identifier, nameof(identifier));
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .FirstOrDefaultAsync(x => x.Identifier == identifier, GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Find expected by identifier
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="expectedId"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<SecurityCredentialInfo> FindExpectedByIdentifierAsync(
            string identifier,
            Guid? expectedId = null,
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .WhereIf(!identifier.IsNullOrWhiteSpace(), x => x.Identifier == identifier)
                .WhereIf(expectedId.HasValue, x => x.Id != expectedId.Value)
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

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
        public virtual async Task<List<SecurityCredentialInfo>> GetListAsync(
            string sorting = null,
            string identifier = "",
            string keyType = "",
            string bizType = "",
            DateTime? expiresMin = null,
            DateTime? expiresMax = null,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!identifier.IsNullOrWhiteSpace(), item => item.Identifier == identifier)
                .WhereIf(!keyType.IsNullOrWhiteSpace(), item => item.KeyType == keyType)
                .WhereIf(!bizType.IsNullOrWhiteSpace(), item => item.BizType == bizType)
                .WhereIf(expiresMin.HasValue, item => item.Expires >= expiresMin.Value)
                .WhereIf(expiresMax.HasValue, item => item.Expires < expiresMax.Value)
                .OrderBy(sorting ?? nameof(SecurityCredentialInfo.Id))
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

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
        public virtual async Task<List<SecurityCredentialInfo>> GetPagedListAsync(
            int skipCount,
            int maxResultCount,
            string sorting = null,
            string identifier = "",
            string keyType = "",
            string bizType = "",
            DateTime? expiresMin = null,
            DateTime? expiresMax = null,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!identifier.IsNullOrWhiteSpace(), item => item.Identifier == identifier)
                .WhereIf(!keyType.IsNullOrWhiteSpace(), item => item.KeyType == keyType)
                .WhereIf(!bizType.IsNullOrWhiteSpace(), item => item.BizType == bizType)
                .WhereIf(expiresMin.HasValue, item => item.Expires >= expiresMin.Value)
                .WhereIf(expiresMax.HasValue, item => item.Expires < expiresMax.Value)
                .OrderBy(sorting ?? nameof(SecurityCredentialInfo.Id))
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

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
        public virtual async Task<int> GetCountAsync(
            string identifier = "",
            string keyType = "",
            string bizType = "",
            DateTime? expiresMin = null,
            DateTime? expiresMax = null,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!identifier.IsNullOrWhiteSpace(), item => item.Identifier == identifier)
                .WhereIf(!keyType.IsNullOrWhiteSpace(), item => item.KeyType == keyType)
                .WhereIf(!bizType.IsNullOrWhiteSpace(), item => item.BizType == bizType)
                .WhereIf(expiresMin.HasValue, item => item.Expires >= expiresMin.Value)
                .WhereIf(expiresMax.HasValue, item => item.Expires < expiresMax.Value)
                .CountAsync(GetCancellationToken(cancellationToken));
        }
    }
}
