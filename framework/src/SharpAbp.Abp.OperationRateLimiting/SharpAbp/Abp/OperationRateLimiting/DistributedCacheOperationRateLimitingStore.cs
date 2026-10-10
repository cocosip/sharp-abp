using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DistributedLocking;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Timing;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public class DistributedCacheOperationRateLimitingStore : IOperationRateLimitingStore, ITransientDependency
    {
        protected IDistributedCache<OperationRateLimitingCacheItem> Cache { get; }
        protected IAbpDistributedLock DistributedLock { get; }
        protected IClock Clock { get; }
        protected OperationRateLimitingConfiguration Configuration { get; }

        public DistributedCacheOperationRateLimitingStore(IDistributedCache<OperationRateLimitingCacheItem> cache,
            IAbpDistributedLock distributedLock, IClock clock, OperationRateLimitingConfiguration configuration)
        {
            Cache = cache;
            DistributedLock = distributedLock;
            Clock = clock;
            Configuration = configuration;
        }

        protected virtual DateTime GetUtcNow()
        {
            var now = Clock.Now;
            return now.Kind == DateTimeKind.Utc ? now : now.ToUniversalTime();
        }

        protected virtual async Task<IAbpDistributedLockHandle> AcquireAsync(string key, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var handle = await DistributedLock.TryAcquireAsync(key + ":lock", Configuration.LockTimeout, cancellationToken).ConfigureAwait(false);
            if (handle == null)
            {
                throw new TimeoutException("Operation rate counter lock acquisition timed out.");
            }
            return handle;
        }

        public virtual async Task<OperationRateLimitingStoreResult> IncrementAsync(string key, TimeSpan duration,
            int maxCount, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(key) || duration <= TimeSpan.Zero || maxCount < 0)
            {
                throw new ArgumentException("A counter requires a key, positive duration and nonnegative maximum.");
            }

            await using (var handle = await AcquireAsync(key, cancellationToken).ConfigureAwait(false))
            {
                var state = await GetAsync(key, cancellationToken).ConfigureAwait(false);
                if (state.CurrentCount >= maxCount)
                {
                    return new OperationRateLimitingStoreResult(false, state);
                }

                var end = state.WindowEndUtc ?? GetUtcNow().Add(duration);
                var item = new OperationRateLimitingCacheItem
                {
                    CurrentCount = state.CurrentCount + 1,
                    WindowEndUtc = end
                };
                await Cache.SetAsync(key, item, new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = new DateTimeOffset(end)
                }, hideErrors: false, considerUow: false, token: cancellationToken).ConfigureAwait(false);
                return new OperationRateLimitingStoreResult(true,
                    new OperationRateLimitingStoreState(item.CurrentCount, end));
            }
        }

        public virtual async Task<OperationRateLimitingStoreState> GetAsync(string key, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var item = await Cache.GetAsync(key, hideErrors: false, considerUow: false, token: cancellationToken).ConfigureAwait(false);
            if (item == null || item.WindowEndUtc <= GetUtcNow())
            {
                return new OperationRateLimitingStoreState();
            }
            if (item.CurrentCount < 0 || item.WindowEndUtc.Kind != DateTimeKind.Utc)
            {
                throw new InvalidOperationException("Invalid operation counter state.");
            }
            return new OperationRateLimitingStoreState(item.CurrentCount, item.WindowEndUtc);
        }

        public virtual async Task ResetAsync(string key, CancellationToken cancellationToken = default)
        {
            await using (var handle = await AcquireAsync(key, cancellationToken).ConfigureAwait(false))
            {
                await Cache.RemoveAsync(key, hideErrors: false, considerUow: false, token: cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
