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

namespace SharpAbp.Abp.CryptoVault.EntityFrameworkCore
{
    public class EfCoreSM2CredsRepository : EfCoreRepository<IAbpCryptoVaultDbContext, SM2Creds, Guid>, ISM2CredsRepository
    {
        public EfCoreSM2CredsRepository(IDbContextProvider<IAbpCryptoVaultDbContext> dbContextProvider)
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
        public virtual async Task<SM2Creds> FindByIdentifierAsync(
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
        public virtual async Task<SM2Creds> FindExpectedByIdentifierAsync(
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
        ///  Get list
        /// </summary>
        /// <param name="sorting"></param>
        /// <param name="identifier"></param>
        /// <param name="sourceType"></param>
        /// <param name="curve"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<List<SM2Creds>> GetListAsync(
            string sorting = null,
            string identifier = "",
            int? sourceType = null,
            string curve = "",
            bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!identifier.IsNullOrWhiteSpace(), item => item.Identifier == identifier)
                .WhereIf(!curve.IsNullOrWhiteSpace(), item => item.Curve == curve)
                .OrderBy(sorting ?? nameof(SM2Creds.Id))
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

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
        public virtual async Task<List<SM2Creds>> GetPagedListAsync(
            int skipCount,
            int maxResultCount,
            string sorting = null,
            string identifier = "",
            int? sourceType = null,
            string curve = "",
            bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!identifier.IsNullOrWhiteSpace(), item => item.Identifier == identifier)
                .WhereIf(sourceType.HasValue, item => item.SourceType == sourceType.Value)
                .WhereIf(!curve.IsNullOrWhiteSpace(), item => item.Curve == curve)
                .OrderBy(sorting ?? nameof(RSACreds.Id))
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Get count
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="sourceType"></param>
        /// <param name="curve"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<int> GetCountAsync(
            string identifier = "",
            int? sourceType = null,
            string curve = "",
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!identifier.IsNullOrWhiteSpace(), item => item.Identifier == identifier)
                .WhereIf(sourceType.HasValue, item => item.SourceType == sourceType.Value)
                .WhereIf(!curve.IsNullOrWhiteSpace(), item => item.Curve == curve)
                .CountAsync(GetCancellationToken(cancellationToken));
        }
    }
}
