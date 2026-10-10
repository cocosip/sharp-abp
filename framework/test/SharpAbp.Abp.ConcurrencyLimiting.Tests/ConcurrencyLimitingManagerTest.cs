using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Volo.Abp;
using System.Collections;
using System.Reflection;


namespace SharpAbp.Abp.ConcurrencyLimiting
{

    public class ConcurrencyLimitingManagerTest
    {
        [Fact]
        public void Global_immediate_facade_matches_singleton_limiter_busy_contract()
        {
            using var services = TestServices.Create(new SharpAbpConcurrencyLimitingOptions()
                .AddPolicy("Qido", builder => builder.WithConcurrency(1, 0).PartitionGlobally()));
            using var firstScope = services.CreateScope();
            using var secondScope = services.CreateScope();
            var first = firstScope.ServiceProvider.GetRequiredService<IConcurrencyLimitingManager>();
            var second = secondScope.ServiceProvider.GetRequiredService<IConcurrencyLimitingManager>();
            using var held = EnterQido(first);
            var busy = Assert.Throws<BusinessException>(() => EnterQido(second));
            Assert.Equal("DicomWeb:DicomWebBusy", busy.Code);
            Assert.Equal(0, second.GetStatistics("Qido").QueuedOperations);
            held.Dispose();
            using var next = EnterQido(second);
            Assert.True(next.IsAcquired);
        }

        private static ConcurrencyLimitingLease EnterQido(IConcurrencyLimitingManager manager)
        {
            var lease = manager.TryAcquire("Qido");
            if (lease.IsAcquired)
            {
                return lease;
            }
            lease.Dispose();
            throw new BusinessException("DicomWeb:DicomWebBusy");
        }

        private static SharpAbpConcurrencyLimitingOptions Options(int queue = 0) => new SharpAbpConcurrencyLimitingOptions().AddPolicy("p", b => b.WithConcurrency(1, queue));

        [Fact]
        public void Permit_lifetime_and_repeated_disposal_do_not_inflate_capacity()
        {
            using var services = TestServices.Create(Options());
            var manager = services.GetRequiredService<IConcurrencyLimitingManager>();
            using var first = manager.TryAcquire("p");
            Assert.True(first.IsAcquired);
            using var denied = manager.TryAcquire("p");
            Assert.False(denied.IsAcquired);
            first.Dispose();
            first.Dispose();
            denied.Dispose();
            using var second = manager.TryAcquire("p");
            using var third = manager.TryAcquire("p");
            Assert.True(second.IsAcquired);
            Assert.False(third.IsAcquired);
        }

        [Fact]
        public async Task Bounded_fifo_queue_and_immediate_attempt()
        {
            using var services = TestServices.Create(Options(2));
            var manager = services.GetRequiredService<IConcurrencyLimitingManager>();
            using var held = manager.TryAcquire("p");
            var one = manager.AcquireAsync("p");
            var two = manager.AcquireAsync("p");
            Assert.False(one.IsCompleted);
            Assert.False(two.IsCompleted);
            Assert.Equal(2, manager.GetStatistics("p").QueuedOperations);
            using var denied = manager.TryAcquire("p");
            Assert.False(denied.IsAcquired);
            Assert.Equal(2, manager.GetStatistics("p").QueuedOperations);
            var full = await Assert.ThrowsAsync<ConcurrencyLimitExceededException>(() => manager.AcquireAsync("p"));
            Assert.Equal(ConcurrencyLimitExceededReason.QueueFull, full.Reason);
            held.Dispose();
            using var first = await one.WaitAsync(TimeSpan.FromSeconds(5));
            Assert.False(two.IsCompleted);
            first.Dispose();
            using var second = await two.WaitAsync(TimeSpan.FromSeconds(5));
            Assert.True(second.IsAcquired);
            Assert.Equal(0, manager.GetStatistics("p").AvailablePermits);
        }

        [Fact]
        public async Task Execute_holds_permit_until_business_task_completes_and_releases_after_failure()
        {
            using var services = TestServices.Create(Options());
            var manager = services.GetRequiredService<IConcurrencyLimitingManager>();
            var entered = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var finish = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var running = manager.ExecuteAsync("p", async _ => { entered.SetResult(true); await finish.Task; });
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5));
            using (var denied = manager.TryAcquire("p"))
                Assert.False(denied.IsAcquired);
            finish.SetResult(true);
            await running;
            await Assert.ThrowsAsync<InvalidOperationException>(() => manager.ExecuteAsync("p", _ => Task.FromException(new InvalidOperationException())));
            await Assert.ThrowsAsync<InvalidOperationException>(() => manager.ExecuteAsync<int>("p", _ => Task.FromException<int>(new InvalidOperationException())));
            Assert.Equal(42, await manager.ExecuteAsync("p", _ => Task.FromResult(42)));
            Assert.Equal(1, manager.GetStatistics("p").AvailablePermits);
        }

        [Fact]
        public async Task Caller_cancellation_preserves_exception_and_timeout_is_distinct()
        {
            using var services = TestServices.Create(Options(1).ConfigurePolicy("p", b => b.WithQueueTimeout(TimeSpan.FromMilliseconds(50))));
            var manager = services.GetRequiredService<IConcurrencyLimitingManager>();
            using var held = manager.TryAcquire("p");
            using var cancel = new CancellationTokenSource();
            var queued = manager.AcquireAsync("p", cancellationToken: cancel.Token);
            cancel.Cancel();
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => queued);
            Assert.Equal(0, manager.GetStatistics("p").QueuedOperations);
            var timeout = await Assert.ThrowsAsync<ConcurrencyLimitExceededException>(() => manager.AcquireAsync("p"));
            Assert.Equal(ConcurrencyLimitExceededReason.QueueTimeout, timeout.Reason);
            Assert.Equal("p", timeout.PolicyName);
            Assert.Equal(1, timeout.PermitLimit);
            Assert.Equal(1, timeout.QueueLimit);
            Assert.Equal(0, manager.GetStatistics("p").QueuedOperations);
        }

        [Fact]
        public async Task Pre_cancelled_execution_never_enters_business()
        {
            using var services = TestServices.Create(Options());
            var manager = services.GetRequiredService<IConcurrencyLimitingManager>();
            using var cancel = new CancellationTokenSource();
            cancel.Cancel();
            var entered = false;
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => manager.ExecuteAsync("p", _ => { entered = true; return Task.CompletedTask; }, cancellationToken: cancel.Token));
            Assert.False(entered);
            Assert.Equal(1, manager.GetStatistics("p").AvailablePermits);
        }

        [Fact]
        public async Task Cancellation_racing_release_does_not_leak_permits()
        {
            using var services = TestServices.Create(Options(1));
            var manager = services.GetRequiredService<IConcurrencyLimitingManager>();
            for (var i = 0; i < 100; i++)
            {
                using var held = manager.TryAcquire("p");
                using var cancel = new CancellationTokenSource();
                var pending = manager.AcquireAsync("p", cancellationToken: cancel.Token);
                await Task.WhenAll(Task.Run(() => cancel.Cancel()), Task.Run(() => held.Dispose()));
                try
                {
                    using var acquired = await pending;
                }
                catch (OperationCanceledException) { }
                Assert.Equal(1, manager.GetStatistics("p").AvailablePermits);
                Assert.Equal(0, manager.GetStatistics("p").QueuedOperations);
            }
        }

        [Fact]
        public async Task Capacity_rejection_disabled_mode_and_invalid_arguments()
        {
            using var services = TestServices.Create(Options());
            var manager = services.GetRequiredService<IConcurrencyLimitingManager>();
            using var held = manager.TryAcquire("p");
            Assert.Equal(ConcurrencyLimitExceededReason.Capacity, (await Assert.ThrowsAsync<ConcurrencyLimitExceededException>(() => manager.AcquireAsync("p"))).Reason);
            Assert.Throws<ConcurrencyLimitingConfigurationException>(() => manager.TryAcquire("missing"));
            using var disabledServices = TestServices.Create(new SharpAbpConcurrencyLimitingOptions { IsEnabled = false });
            var disabled = disabledServices.GetRequiredService<IConcurrencyLimitingManager>();
            using var noOp = await disabled.AcquireAsync("unknown");
            Assert.True(noOp.IsAcquired);
            Assert.False(disabled.GetStatistics("unknown").IsEnabled);
            Assert.Equal(int.MaxValue, disabled.GetStatistics("unknown").AvailablePermits);
            Assert.Equal(0, disabled.GetStatistics("unknown").QueuedOperations);
            var registry = disabledServices.GetRequiredService<ConcurrencyLimiterRegistry>();
            var nativeLimiters = (IDictionary)typeof(ConcurrencyLimiterRegistry)
                .GetField("_limiters", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(registry)!;
            Assert.Empty(nativeLimiters);
            Assert.Throws<ArgumentException>(() => disabled.TryAcquire(" "));
            await Assert.ThrowsAsync<ArgumentNullException>(() => disabled.ExecuteAsync("unknown", (Func<CancellationToken, Task>)null!));
        }

        [Fact]
        public async Task Cancellation_of_business_returns_permit()
        {
            using var services = TestServices.Create(Options());
            var manager = services.GetRequiredService<IConcurrencyLimitingManager>();
            using var cancel = new CancellationTokenSource();
            var entered = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var running = manager.ExecuteAsync("p", async token => { entered.SetResult(true); await Task.Delay(Timeout.Infinite, token); }, cancellationToken: cancel.Token);
            await entered.Task;
            cancel.Cancel();
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => running);
            Assert.Equal(1, manager.GetStatistics("p").AvailablePermits);
        }
    }

}
