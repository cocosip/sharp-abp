using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.RateLimiting;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace SharpAbp.Abp.ConcurrencyLimiting
{
    public class ConcurrencyLimiterRegistryTest
    {
        [Fact]
        public async Task Native_cleanup_evicts_idle_partitions_and_retains_active_and_queued_capacity()
        {
            using var services = TestServices.Create(new SharpAbpConcurrencyLimitingOptions()
                .AddPolicy("p", builder => builder.WithConcurrency(1, 1).PartitionByParameter()));
            var registry = services.GetRequiredService<ConcurrencyLimiterRegistry>();
            var policy = services.GetRequiredService<IConcurrencyLimitingPolicyProvider>().Get("p");
            var limiter = registry.Get(policy);
            var activeKey = new ConcurrencyLimitingPartitionKey("active", null);
            using var held = limiter.AttemptAcquire(activeKey);
            var queued = limiter.AcquireAsync(activeKey).AsTask();
            Assert.False(queued.IsCompleted);

            for (var i = 0; i < 100; i++)
            {
                using var idle = limiter.AttemptAcquire(new ConcurrencyLimitingPartitionKey("idle-" + i, null));
                Assert.True(idle.IsAcquired);
            }

            // Test-only control of native idle timestamps; the real native timer owns cleanup.
            // System.Threading.RateLimiting 10.0.15 wraps the limiter in a LimiterEntry that carries the
            // last access timestamp, earlier versions store the lazily created limiter directly.
            var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            var nativeType = limiter.GetType();
            var partitions = (System.Collections.IDictionary)nativeType.GetField("_limiters", flags)!.GetValue(limiter)!;
            var idleTimestamp = Stopwatch.GetTimestamp() - 20 * Stopwatch.Frequency;
            lock (partitions.SyncRoot)
            {
                Assert.Equal(101, partitions.Count);
                foreach (System.Collections.DictionaryEntry partition in partitions)
                {
                    var entry = partition.Value!.GetType().GetProperty("Value", flags)!.GetValue(partition.Value)!;
                    // Access timestamp is only used for the no-op limiter, the idle duration of a concrete
                    // limiter comes from its own _idleSince field.
                    entry.GetType().GetField("LastAccessTimestamp", flags)?.SetValue(entry, idleTimestamp);
                    var rateLimiter = entry.GetType().GetProperty("Limiter", flags)?.GetValue(entry) as RateLimiter
                        ?? entry as RateLimiter;
                    if (rateLimiter?.IdleDuration.HasValue == true)
                    {
                        rateLimiter.GetType().GetField("_idleSince", flags)!.SetValue(rateLimiter, idleTimestamp);
                    }
                }
            }
            var deadline = Stopwatch.StartNew();
            while (true)
            {
                lock (partitions.SyncRoot)
                {
                    if (partitions.Count == 1)
                    {
                        Assert.True(partitions.Contains(activeKey));
                        break;
                    }
                }
                Assert.True(deadline.Elapsed < TimeSpan.FromSeconds(5), "Native idle cleanup did not remove idle partitions.");
                await Task.Delay(10);
            }
            Assert.False(queued.IsCompleted);
            using (var denied = limiter.AttemptAcquire(activeKey))
            {
                Assert.False(denied.IsAcquired);
            }
            held.Dispose();
            using var released = await queued.WaitAsync(TimeSpan.FromSeconds(5));
            Assert.True(released.IsAcquired);
        }

        [Fact]
        public void Same_partition_shares_across_scopes_and_active_partition_stays_alive()
        {
            using var services = TestServices.Create(new SharpAbpConcurrencyLimitingOptions()
                .AddPolicy("p", b => b.WithConcurrency(1).PartitionByParameter())
                .AddPolicy("other", b => b.WithConcurrency(1).PartitionByParameter()));
            using var first = services.CreateScope();
            using var second = services.CreateScope();
            var registry = services.GetRequiredService<ConcurrencyLimiterRegistry>();
            var policies = services.GetRequiredService<IConcurrencyLimitingPolicyProvider>();
            var p = policies.Get("p");
            var context = new ConcurrencyLimitingContext { Parameter = "one" };
            var k1 = first.ServiceProvider.GetRequiredService<ConcurrencyLimitingPartitionResolver>().Resolve(p, context);
            var k2 = second.ServiceProvider.GetRequiredService<ConcurrencyLimitingPartitionResolver>().Resolve(p, context);
            var limiter = registry.Get(p);
            using var lease = limiter.AttemptAcquire(k1);
            Assert.True(lease.IsAcquired);
            for (var i = 0; i < 100; i++)
            {
                using var denied = registry.Get(p).AttemptAcquire(k2);
                Assert.False(denied.IsAcquired);
            }
            using var otherPartition = limiter.AttemptAcquire(first.ServiceProvider.GetRequiredService<ConcurrencyLimitingPartitionResolver>().Resolve(p, new ConcurrencyLimitingContext { Parameter = "two" }));
            using var otherPolicy = registry.Get(policies.Get("other")).AttemptAcquire(k1);
            Assert.True(otherPartition.IsAcquired);
            Assert.True(otherPolicy.IsAcquired);
        }

        [Fact]
        public async Task Registry_shutdown_releases_waiters_and_rejects_new_access()
        {
            using var services = TestServices.Create(new SharpAbpConcurrencyLimitingOptions().AddPolicy("p", b => b.WithConcurrency(1, 1)));
            var registry = services.GetRequiredService<ConcurrencyLimiterRegistry>();
            var policy = services.GetRequiredService<IConcurrencyLimitingPolicyProvider>().Get("p");
            var key = services.GetRequiredService<ConcurrencyLimitingPartitionResolver>().Resolve(policy, new ConcurrencyLimitingContext());
            var limiter = registry.Get(policy);
            using var lease = limiter.AttemptAcquire(key);
            var pending = limiter.AcquireAsync(key).AsTask();
            Assert.False(pending.IsCompleted);
            registry.Dispose();
            using var failed = await pending.WaitAsync(TimeSpan.FromSeconds(5));
            Assert.False(failed.IsAcquired);
            Assert.Throws<ObjectDisposedException>(() => registry.Get(policy));
        }
    }

}
