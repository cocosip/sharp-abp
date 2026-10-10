using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.OperationRateLimiting.Fakes;
using Volo.Abp.Caching;
using Volo.Abp.DistributedLocking;
using Volo.Abp.MultiTenancy;
using Xunit;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public class OperationRateLimitingStoreTest : SharpAbpOperationRateLimitingTestBase
    {
        [Fact]
        public async Task Last_permit_and_contended_increment_are_atomic()
        {
            var store = GetRequiredService<IOperationRateLimitingStore>();
            var results = await Task.WhenAll(Enumerable.Range(0, 100)
                .Select(_ => store.IncrementAsync("same-key", TimeSpan.FromMinutes(1), 1)));
            Assert.Equal(1, results.Count(result => result.IsAccepted));
            Assert.All(results, result => Assert.Equal(1, result.State.CurrentCount));
            Assert.False((await store.IncrementAsync("ban", TimeSpan.FromMinutes(1), 0)).IsAccepted);
            Assert.Null((await store.GetAsync("ban")).WindowEndUtc);
        }

        [Fact]
        public async Task Window_expires_logically_at_deadline_without_access_renewal()
        {
            var store = GetRequiredService<IOperationRateLimitingStore>();
            var clock = GetRequiredService<MutableClock>();
            var cache = GetRequiredService<ControllableCache>();
            Assert.Null((await store.GetAsync("key")).WindowEndUtc);
            Assert.Equal(0, cache.Count);
            var first = await store.IncrementAsync("key", TimeSpan.FromMinutes(1), 3);
            clock.Now = clock.Now.AddSeconds(59);
            var second = await store.IncrementAsync("key", TimeSpan.FromHours(1), 3);
            Assert.Equal(first.State.WindowEndUtc, second.State.WindowEndUtc);
            Assert.All(cache.Deadlines.Values, deadline => Assert.Equal(new DateTimeOffset(first.State.WindowEndUtc!.Value), deadline));
            clock.Now = clock.Now.AddSeconds(1);
            Assert.Equal(0, (await store.GetAsync("key")).CurrentCount);
            Assert.Equal(1, cache.Count);
            var next = await store.IncrementAsync("key", TimeSpan.FromHours(1), 3);
            Assert.Equal(1, next.State.CurrentCount);
            Assert.Equal(clock.Now.AddHours(1), next.State.WindowEndUtc);
        }

        [Fact]
        public async Task Cache_faults_cancellation_and_lock_timeout_remain_infrastructure_errors()
        {
            var store = GetRequiredService<IOperationRateLimitingStore>();
            var cache = GetRequiredService<ControllableCache>();
            cache.FailRead = true;
            await Assert.ThrowsAsync<IOException>(() => store.GetAsync("key"));
            await Assert.ThrowsAsync<IOException>(() => store.IncrementAsync("key", TimeSpan.FromMinutes(1), 2));
            cache.FailRead = false;
            cache.FailWrite = true;
            await Assert.ThrowsAsync<IOException>(() => store.IncrementAsync("key", TimeSpan.FromMinutes(1), 2));
            cache.FailWrite = false;
            cache.FailRemove = true;
            await Assert.ThrowsAsync<IOException>(() => store.ResetAsync("key"));
            using (var cancelled = new CancellationTokenSource())
            {
                cancelled.Cancel();
                await Assert.ThrowsAnyAsync<OperationCanceledException>(() => store.GetAsync("key", cancelled.Token));
                await Assert.ThrowsAnyAsync<OperationCanceledException>(() => store.ResetAsync("key", cancelled.Token));
            }
            var timedOut = new DistributedCacheOperationRateLimitingStore(GetRequiredService<IDistributedCache<OperationRateLimitingCacheItem>>(),
                new FailingLock(), GetRequiredService<MutableClock>(), GetRequiredService<OperationRateLimitingConfiguration>());
            await Assert.ThrowsAsync<TimeoutException>(() => timedOut.IncrementAsync("key", TimeSpan.FromMinutes(1), 1));
        }

        [Fact]
        public async Task Global_counter_is_shared_across_tenants_and_reset_uses_increment_lock()
        {
            var checker = GetRequiredService<IOperationRateLimitingChecker>();
            var tenant = GetRequiredService<ICurrentTenant>();
            using (tenant.Change(Guid.NewGuid()))
            {
                await checker.CheckAsync("one");
            }
            using (tenant.Change(Guid.NewGuid()))
            {
                Assert.False(await checker.IsAllowedAsync("one"));
            }
            var store = GetRequiredService<IOperationRateLimitingStore>();
            var distributedLock = GetRequiredService<IAbpDistributedLock>();
            var handle = await distributedLock.TryAcquireAsync("race:lock", TimeSpan.FromSeconds(1));
            var increment = store.IncrementAsync("race", TimeSpan.FromMinutes(1), 5);
            var reset = store.ResetAsync("race");
            await Task.Delay(30);
            Assert.False(increment.IsCompleted);
            Assert.False(reset.IsCompleted);
            await handle!.DisposeAsync();
            await Task.WhenAll(increment, reset);
            var state = await store.GetAsync("race");
            Assert.InRange(state.CurrentCount, 0, 1);
        }
    }

    public class FailingLock : IAbpDistributedLock
    {
        public Task<IAbpDistributedLockHandle?> TryAcquireAsync(string name, TimeSpan timeout = default,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IAbpDistributedLockHandle?>(null);
        }
    }
}
