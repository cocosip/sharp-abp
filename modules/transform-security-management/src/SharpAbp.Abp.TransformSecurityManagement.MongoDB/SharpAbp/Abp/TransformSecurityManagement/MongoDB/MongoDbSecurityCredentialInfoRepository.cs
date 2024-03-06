﻿using JetBrains.Annotations;
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

namespace SharpAbp.Abp.TransformSecurityManagement.MongoDB
{
    public class MongoDbSecurityCredentialInfoRepository : MongoDbRepository<IAbpTransformSecurityManagementMongoDbContext, SecurityCredentialInfo, Guid>, ISecurityCredentialInfoRepository
    {
        public MongoDbSecurityCredentialInfoRepository(IMongoDbContextProvider<IAbpTransformSecurityManagementMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task<SecurityCredentialInfo> FindByIdentifierAsync(
            [NotNull] string identifier,
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(identifier, nameof(identifier));
            return await FindAsync(x => x.Identifier == identifier, includeDetails, GetCancellationToken(cancellationToken));
        }

        public virtual async Task<SecurityCredentialInfo> FindExpectedByIdentifierAsync(
            string identifier,
            Guid? expectedId = null,
            bool includeDetails = true,
            CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync())
                .WhereIf(!identifier.IsNullOrWhiteSpace(), x => x.Identifier == identifier)
                .WhereIf(expectedId.HasValue, x => x.Id != expectedId.Value)
                .As<IMongoQueryable<SecurityCredentialInfo>>()
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<List<SecurityCredentialInfo>> GetListAsync(
            string sorting = null,
            string identifier = "",
            string keyType = "",
            string bizType = "",
            DateTime? expiresMin = null,
            DateTime? expiresMax = null,
            CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync())
                .WhereIf<SecurityCredentialInfo, IMongoQueryable<SecurityCredentialInfo>>(!identifier.IsNullOrWhiteSpace(), item => item.Identifier == identifier)
                .WhereIf<SecurityCredentialInfo, IMongoQueryable<SecurityCredentialInfo>>(!keyType.IsNullOrWhiteSpace(), item => item.KeyType == keyType)
                .WhereIf<SecurityCredentialInfo, IMongoQueryable<SecurityCredentialInfo>>(!bizType.IsNullOrWhiteSpace(), item => item.BizType == bizType)
                .WhereIf<SecurityCredentialInfo, IMongoQueryable<SecurityCredentialInfo>>(expiresMin.HasValue, item => item.Expires >= expiresMin.Value)
                .WhereIf<SecurityCredentialInfo, IMongoQueryable<SecurityCredentialInfo>>(expiresMax.HasValue, item => item.Expires < expiresMax.Value)
                .OrderBy(sorting ?? nameof(SecurityCredentialInfo.Id))
                .As<IMongoQueryable<SecurityCredentialInfo>>()
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

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
            return await (await GetMongoQueryableAsync())
                .WhereIf<SecurityCredentialInfo, IMongoQueryable<SecurityCredentialInfo>>(!identifier.IsNullOrWhiteSpace(), item => item.Identifier == identifier)
                .WhereIf<SecurityCredentialInfo, IMongoQueryable<SecurityCredentialInfo>>(!keyType.IsNullOrWhiteSpace(), item => item.KeyType == keyType)
                .WhereIf<SecurityCredentialInfo, IMongoQueryable<SecurityCredentialInfo>>(!bizType.IsNullOrWhiteSpace(), item => item.BizType == bizType)
                .WhereIf<SecurityCredentialInfo, IMongoQueryable<SecurityCredentialInfo>>(expiresMin.HasValue, item => item.Expires >= expiresMin.Value)
                .WhereIf<SecurityCredentialInfo, IMongoQueryable<SecurityCredentialInfo>>(expiresMax.HasValue, item => item.Expires < expiresMax.Value)
                .OrderBy(sorting ?? nameof(SecurityCredentialInfo.Id))
                .As<IMongoQueryable<SecurityCredentialInfo>>()
                .PageBy<SecurityCredentialInfo, IMongoQueryable<SecurityCredentialInfo>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }


        public virtual async Task<int> GetCountAsync(
            string identifier = "", 
            string keyType = "",
            string bizType = "", 
            DateTime? expiresMin = null,
            DateTime? expiresMax = null, 
            CancellationToken cancellationToken = default)
        {
            return await(await GetMongoQueryableAsync())
                .WhereIf<SecurityCredentialInfo, IMongoQueryable<SecurityCredentialInfo>>(!identifier.IsNullOrWhiteSpace(), item => item.Identifier == identifier)
                .WhereIf<SecurityCredentialInfo, IMongoQueryable<SecurityCredentialInfo>>(!keyType.IsNullOrWhiteSpace(), item => item.KeyType == keyType)
                .WhereIf<SecurityCredentialInfo, IMongoQueryable<SecurityCredentialInfo>>(!bizType.IsNullOrWhiteSpace(), item => item.BizType == bizType)
                .WhereIf<SecurityCredentialInfo, IMongoQueryable<SecurityCredentialInfo>>(expiresMin.HasValue, item => item.Expires >= expiresMin.Value)
                .WhereIf<SecurityCredentialInfo, IMongoQueryable<SecurityCredentialInfo>>(expiresMax.HasValue, item => item.Expires < expiresMax.Value)
                .As<IMongoQueryable<SecurityCredentialInfo>>()
                .CountAsync(GetCancellationToken(cancellationToken));
        }



    }
}
