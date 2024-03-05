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
    public class MongoDbSM2CredsRepository : MongoDbRepository<IAbpCryptoVaultMongoDbContext, SM2Creds, Guid>, ISM2CredsRepository
    {
        public MongoDbSM2CredsRepository(IMongoDbContextProvider<IAbpCryptoVaultMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public virtual async Task<SM2Creds> FindByIdentifierAsync(
            [NotNull] string identifier,
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(identifier, nameof(identifier));
            return await FindAsync(x => x.Identifier == identifier, includeDetails, GetCancellationToken(cancellationToken));
        }

        public virtual async Task<SM2Creds> FindExpectedByIdentifierAsync(
            string identifier,
            Guid? expectedId = null,
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync())
                .WhereIf(!identifier.IsNullOrWhiteSpace(), x => x.Identifier == identifier)
                .WhereIf(expectedId.HasValue, x => x.Id != expectedId.Value)
                .As<IMongoQueryable<SM2Creds>>()
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<SM2Creds> GetRandomAsync(
            int? sourceType = null,
            string curve = "",
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync())
                .WhereIf(sourceType.HasValue, item => item.SourceType == sourceType.Value)
                .WhereIf(!curve.IsNullOrWhiteSpace(), item => item.Curve == curve)
                .OrderBy(x => Guid.NewGuid())
                .As<IMongoQueryable<SM2Creds>>()
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<List<SM2Creds>> GetListAsync(
            string sorting = null,
            string identifier = "",
            int? sourceType = null,
            string curve = "",
            bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync())
                .WhereIf<SM2Creds, IMongoQueryable<SM2Creds>>(!identifier.IsNullOrWhiteSpace(), item => item.Identifier == identifier)
                .WhereIf<SM2Creds, IMongoQueryable<SM2Creds>>(sourceType.HasValue, item => item.SourceType == sourceType.Value)
                .WhereIf<SM2Creds, IMongoQueryable<SM2Creds>>(!curve.IsNullOrWhiteSpace(), item => item.Curve == curve)
                .OrderBy(sorting ?? nameof(SM2Creds.Id))
                .As<IMongoQueryable<SM2Creds>>()
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

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
            return await (await GetMongoQueryableAsync())
                .WhereIf<SM2Creds, IMongoQueryable<SM2Creds>>(!identifier.IsNullOrWhiteSpace(), item => item.Identifier == identifier)
                .WhereIf<SM2Creds, IMongoQueryable<SM2Creds>>(sourceType.HasValue, item => item.SourceType == sourceType.Value)
                .WhereIf<SM2Creds, IMongoQueryable<SM2Creds>>(!curve.IsNullOrWhiteSpace(), item => item.Curve == curve)
                .OrderBy(sorting ?? nameof(SM2Creds.Id))
                .As<IMongoQueryable<SM2Creds>>()
                .PageBy<SM2Creds, IMongoQueryable<SM2Creds>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<int> GetCountAsync(
            string identifier = "",
            int? sourceType = null,
            string curve = "",
            CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync())
                .WhereIf<SM2Creds, IMongoQueryable<SM2Creds>>(!identifier.IsNullOrWhiteSpace(), item => item.Identifier == identifier)
                .WhereIf<SM2Creds, IMongoQueryable<SM2Creds>>(sourceType.HasValue, item => item.SourceType == sourceType.Value)
                .WhereIf<SM2Creds, IMongoQueryable<SM2Creds>>(!curve.IsNullOrWhiteSpace(), item => item.Curve == curve)
                .As<IMongoQueryable<SM2Creds>>()
                .CountAsync(GetCancellationToken(cancellationToken));
        }


    }
}
