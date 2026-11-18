using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;

namespace SharpAbp.Abp.TransformSecurity
{
    public class SecurityCredentialStore : ISecurityCredentialStore, ITransientDependency
    {
        protected IDistributedCache<SecurityCredential> Cache { get; }

        protected IClock Clock { get; }

        public SecurityCredentialStore(
            IDistributedCache<SecurityCredential> cache,
            IClock clock)
        {
            Cache = cache;
            Clock = clock;
        }

        public virtual async Task<SecurityCredential?> GetAsync(string identifier, CancellationToken cancellationToken = default)
        {
            return await Cache.GetAsync(identifier, token: cancellationToken);
        }

        public virtual async Task SetAsync(SecurityCredential credential, CancellationToken cancellationToken = default)
        {
            await Cache.SetAsync(
                credential.Identifier!,
                credential,
                options: CreateCacheOptions(credential),
                token: cancellationToken);
        }

        protected virtual DistributedCacheEntryOptions? CreateCacheOptions(SecurityCredential credential)
        {
            if (!credential.Expires.HasValue)
            {
                return null;
            }

            if (credential.IsExpires(Clock.Now))
            {
                return new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = new DateTimeOffset(Clock.Now)
                };
            }

            return new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = new DateTimeOffset(credential.Expires.Value)
            };
        }
    }
}
