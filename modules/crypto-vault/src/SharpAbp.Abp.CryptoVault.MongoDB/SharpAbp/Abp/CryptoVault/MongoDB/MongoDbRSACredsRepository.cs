using JetBrains.Annotations;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.CryptoVault.MongoDB
{
    public class MongoDbRSACredsRepository : MongoDbRepository<IAbpCryptoVaultMongoDbContext, RSACreds, Guid>, IRSACredsRepository
    {
        public MongoDbRSACredsRepository(IMongoDbContextProvider<IAbpCryptoVaultMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }


        public virtual async Task<RSACreds> FindByIdentifierAsync(
            [NotNull] string identifier,
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(identifier, nameof(identifier));
            return await FindAsync(x => x.Identifier == identifier, includeDetails, GetCancellationToken(cancellationToken));
        }

        public virtual async Task<RSACreds> FindExpectedByIdentifierAsync(
            string identifier,
            Guid? expectedId = null,
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            return await (await GetQueryableAsync())
                .WhereIf(!identifier.IsNullOrWhiteSpace(), x => x.Identifier == identifier)
                .WhereIf(expectedId.HasValue, x => x.Id != expectedId.Value)
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<RSACreds> GetRandomAsync(
            int? sourceType = null,
            int? size = null,
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            return await (await GetQueryableAsync())
                .WhereIf(sourceType.HasValue, item => item.SourceType == sourceType.Value)
                .WhereIf(size.HasValue, item => item.Size == size.Value)
                .OrderBy(x => Guid.NewGuid())
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<List<RSACreds>> GetListAsync(
            string sorting = null,
            string identifier = "",
            int? sourceType = null,
            int? size = null,
            bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            return await (await GetQueryableAsync())
                .WhereIf(!identifier.IsNullOrWhiteSpace(), item => item.Identifier == identifier)
                .WhereIf(sourceType.HasValue, item => item.SourceType == sourceType.Value)
                .WhereIf(size.HasValue, item => item.Size == size.Value)
                .OrderBy(sorting ?? nameof(RSACreds.Id))
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<List<RSACreds>> GetPagedListAsync(
            int skipCount,
            int maxResultCount,
            string sorting = null,
            string identifier = "",
            int? sourceType = null,
            int? size = null,
            bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            return await (await GetQueryableAsync())
                .WhereIf(!identifier.IsNullOrWhiteSpace(), item => item.Identifier == identifier)
                .WhereIf(sourceType.HasValue, item => item.SourceType == sourceType.Value)
                .WhereIf(size.HasValue, item => item.Size == size.Value)
                .OrderBy(sorting ?? nameof(RSACreds.Id))
                .PageBy(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<int> GetCountAsync(
            string identifier = "",
            int? sourceType = null,
            int? size = null,
            CancellationToken cancellationToken = default)
        {
            return await (await GetQueryableAsync())
                .WhereIf(!identifier.IsNullOrWhiteSpace(), item => item.Identifier == identifier)
                .WhereIf(sourceType.HasValue, item => item.SourceType == sourceType.Value)
                .WhereIf(size.HasValue, item => item.Size == size.Value)
                .CountAsync(GetCancellationToken(cancellationToken));
        }

    }
}
