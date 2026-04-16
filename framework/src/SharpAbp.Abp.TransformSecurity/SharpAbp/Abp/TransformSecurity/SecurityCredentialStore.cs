using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.TransformSecurity
{
    public class SecurityCredentialStore : ISecurityCredentialStore, ITransientDependency
    {
        protected IDistributedCache<SecurityCredential> Cache { get; }
        public SecurityCredentialStore(IDistributedCache<SecurityCredential> cache)
        {
            Cache = cache;
        }

        public virtual async Task<SecurityCredential?> GetAsync(string identifier, CancellationToken cancellationToken = default)
        {
            return await Cache.GetAsync(identifier, token: cancellationToken);
        }

        public virtual async Task SetAsync(SecurityCredential credential, CancellationToken cancellationToken = default)
        {
            await Cache.RemoveAsync(credential.Identifier!, token: cancellationToken);
            await Cache.SetAsync(
                credential.Identifier!,
                credential,
                options: CreateCacheOptions(credential),
                token: cancellationToken);
        }

        protected virtual DistributedCacheEntryOptions? CreateCacheOptions(SecurityCredential credential)
        {
            if (!credential.Expires.HasValue || credential.Expires.Value <= DateTime.UtcNow)
            {
                return null;
            }

            return new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = new DateTimeOffset(credential.Expires.Value)
            };
        }
    }
}
