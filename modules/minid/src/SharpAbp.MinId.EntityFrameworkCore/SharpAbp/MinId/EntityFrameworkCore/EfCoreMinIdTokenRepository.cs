using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace SharpAbp.MinId.EntityFrameworkCore
{
    public class EfCoreMinIdTokenRepository : EfCoreRepository<IMinIdDbContext, MinIdToken, Guid>, IMinIdTokenRepository
    {
        public EfCoreMinIdTokenRepository(IDbContextProvider<IMinIdDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        /// <summary>
        /// Find minIdToken by token
        /// </summary>
        /// <param name="bizType"></param>
        /// <param name="token"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<MinIdToken> FindByTokenAsync(
            [NotNull] string bizType,
            [NotNull] string token,
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(bizType, nameof(bizType));
            Check.NotNullOrWhiteSpace(token, nameof(token));
            return await (await GetDbSetAsync())
                .FirstOrDefaultAsync(x => x.BizType == bizType && x.Token == token, GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Find minIdToken by token
        /// </summary>
        /// <param name="bizType"></param>
        /// <param name="token"></param>
        /// <param name="expectedId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<MinIdToken> FindExpectedByTokenAsync(
            string bizType,
            string token,
            Guid? expectedId = null,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!bizType.IsNullOrWhiteSpace(), item => item.BizType == bizType)
                .WhereIf(!token.IsNullOrWhiteSpace(), item => item.Token == token)
                .WhereIf(expectedId.HasValue, item => item.Id != expectedId.Value)
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

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
        public virtual async Task<List<MinIdToken>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string sorting = null,
            string bizType = "",
            string token = "",
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!bizType.IsNullOrWhiteSpace(), item => item.BizType == bizType)
                .WhereIf(!token.IsNullOrWhiteSpace(), item => item.Token == token)
                .OrderBy(sorting ?? nameof(MinIdToken.Id))
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Get count
        /// </summary>
        /// <param name="bizType"></param>
        /// <param name="token"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<int> GetCountAsync(
            string bizType = "",
            string token = "",
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!bizType.IsNullOrWhiteSpace(), item => item.BizType == bizType)
                .WhereIf(!token.IsNullOrWhiteSpace(), item => item.Token == token)
                .CountAsync(GetCancellationToken(cancellationToken));
        }

    }
}
