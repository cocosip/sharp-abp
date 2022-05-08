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

namespace SharpAbp.MinId.EntityFrameworkCore
{
    public class EfCoreMinIdInfoRepository : EfCoreRepository<IMinIdDbContext, MinIdInfo, Guid>, IMinIdInfoRepository
    {
        public EfCoreMinIdInfoRepository(IDbContextProvider<IMinIdDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        /// <summary>
        /// Find minIdInfo by bizType
        /// </summary>
        /// <param name="bizType"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<MinIdInfo> FindByBizTypeAsync([NotNull] string bizType, CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(bizType, nameof(bizType));
            return await (await GetDbSetAsync())
                .FirstOrDefaultAsync(x => x.BizType == bizType, GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Find minIdInfo by bizType
        /// </summary>
        /// <param name="bizType"></param>
        /// <param name="expectedId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<MinIdInfo> FindExpectedByBizTypeAsync(
            string bizType,
            Guid? expectedId = null,
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!bizType.IsNullOrWhiteSpace(), item => item.BizType == bizType)
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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<List<MinIdInfo>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string sorting = null,
            string bizType = "",
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!bizType.IsNullOrWhiteSpace(), item => item.BizType == bizType)
                .OrderBy(sorting ?? nameof(MinIdInfo.Id))
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// Get count
        /// </summary>
        /// <param name="bizType"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<int> GetCountAsync(
            string bizType = "",
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!bizType.IsNullOrWhiteSpace(), item => item.BizType == bizType)
                .CountAsync(GetCancellationToken(cancellationToken));
        }

    }
}
