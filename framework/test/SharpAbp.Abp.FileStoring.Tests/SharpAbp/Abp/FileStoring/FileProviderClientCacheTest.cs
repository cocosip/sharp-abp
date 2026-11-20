using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.FileStoring
{
    public class FileProviderClientCacheTest
    {
        private sealed class DisposableClient : IDisposable
        {
            public string Name { get; }

            public bool IsDisposed { get; private set; }

            private int _disposeCount;
            private readonly bool _throwOnDispose;

            public int DisposeCount => Volatile.Read(ref _disposeCount);

            public DisposableClient(string name, bool throwOnDispose = false)
            {
                Name = name;
                _throwOnDispose = throwOnDispose;
            }

            public void Dispose()
            {
                Interlocked.Increment(ref _disposeCount);
                IsDisposed = true;
                if (_throwOnDispose) { throw new InvalidOperationException(Name); }
            }
        }

        private sealed class DisposingOnRetireCache : FileProviderClientCache<DisposableClient>
        {
            public void SweepExpiredClients()
            {
                Sweep(DateTime.UtcNow.AddHours(1).Ticks);
            }

            protected override void Retire(DisposableClient client, long now)
            {
                Dispose();
                base.Retire(client, now);
            }
        }

        private sealed class ExpiringCache : FileProviderClientCache<DisposableClient>
        {
            public ExpiringCache() : base(TimeSpan.FromMinutes(1), TimeSpan.Zero) { }
            public void Expire() => Sweep(DateTime.UtcNow.AddHours(1).Ticks);
        }

        [Fact]
        public void Retirement_Continues_After_Client_Disposal_Failures()
        {
            using var cache = new ExpiringCache();
            var first = cache.GetOrAdd("first", () => new DisposableClient("first", true));
            var second = cache.GetOrAdd("second", () => new DisposableClient("second", true));
            var healthy = cache.GetOrAdd("healthy", () => new DisposableClient("healthy"));

            var failure = Assert.Throws<AggregateException>(() => cache.Expire());

            Assert.Equal(2, failure.InnerExceptions.Count);
            Assert.Equal(1, first.DisposeCount);
            Assert.Equal(1, second.DisposeCount);
            Assert.Equal(1, healthy.DisposeCount);
        }

        [Fact]
        public async Task Shutdown_Continues_After_Client_Disposal_Failures()
        {
            var cache = new FileProviderClientCache<DisposableClient>();
            var sync = cache.GetOrAdd("sync", () => new DisposableClient("sync", true));
            var asyncClient = await cache.GetOrAddAsync("async", () => Task.FromResult(new DisposableClient("async", true)));
            var healthy = cache.GetOrAdd("healthy", () => new DisposableClient("healthy"));

            var failure = Assert.Throws<AggregateException>(() => cache.Dispose());

            Assert.Equal(2, failure.InnerExceptions.Count);
            Assert.Equal(1, sync.DisposeCount);
            Assert.Equal(1, asyncClient.DisposeCount);
            Assert.Equal(1, healthy.DisposeCount);
            cache.Dispose();
            Assert.Throws<ObjectDisposedException>(() => cache.GetOrAdd("new", () => new DisposableClient("new")));
        }

        [Fact]
        public void GetOrAdd_Should_Return_The_Same_Client_For_The_Same_Key()
        {
            using var cache = new FileProviderClientCache<DisposableClient>();

            var first = cache.GetOrAdd("key", () => new DisposableClient("first"));
            var second = cache.GetOrAdd("key", () => new DisposableClient("second"));

            Assert.Same(first, second);
        }

        [Fact]
        public async Task GetOrAddAsync_Should_Return_The_Same_Client_For_The_Same_Key()
        {
            using var cache = new FileProviderClientCache<DisposableClient>();

            var first = await cache.GetOrAddAsync("key", () => Task.FromResult(new DisposableClient("first")));
            var second = await cache.GetOrAddAsync("key", () => Task.FromResult(new DisposableClient("second")));

            Assert.Same(first, second);
        }

        [Fact]
        public void GetOrAdd_Should_Create_Different_Clients_For_Different_Keys()
        {
            using var cache = new FileProviderClientCache<DisposableClient>();

            var first = cache.GetOrAdd("key1", () => new DisposableClient("first"));
            var second = cache.GetOrAdd("key2", () => new DisposableClient("second"));

            Assert.NotSame(first, second);
        }

        [Fact]
        public void GetOrAdd_Should_Remove_A_Failed_Factory_And_Allow_Retry()
        {
            using var cache = new FileProviderClientCache<DisposableClient>();

            Assert.Throws<InvalidOperationException>(
                () => cache.GetOrAdd("key", () => throw new InvalidOperationException("boom")));

            var client = cache.GetOrAdd("key", () => new DisposableClient("retry"));

            Assert.Equal("retry", client.Name);
        }

        [Fact]
        public async Task GetOrAddAsync_Should_Remove_A_Failed_Factory_And_Allow_Retry()
        {
            using var cache = new FileProviderClientCache<DisposableClient>();

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => cache.GetOrAddAsync("key", () => throw new InvalidOperationException("boom")));

            var client = await cache.GetOrAddAsync("key", () => Task.FromResult(new DisposableClient("retry")));

            Assert.Equal("retry", client.Name);
        }

        [Fact]
        public void Idle_Clients_Should_Be_Disposed_After_The_Grace_Period()
        {
            var idleTimeout = TimeSpan.FromMilliseconds(40);
            using var cache = new FileProviderClientCache<DisposableClient>(idleTimeout, TimeSpan.Zero);

            var client = cache.GetOrAdd("old", () => new DisposableClient("old"));
            Assert.False(client.IsDisposed);

            Thread.Sleep(120);

            // Any subsequent access triggers the sweep.
            cache.GetOrAdd("new", () => new DisposableClient("new"));

            Assert.True(client.IsDisposed);
        }

        [Fact]
        public void Dispose_Should_Dispose_Cached_Clients()
        {
            var cache = new FileProviderClientCache<DisposableClient>();

            var client = cache.GetOrAdd("key", () => new DisposableClient("client"));
            cache.Dispose();

            Assert.True(client.IsDisposed);
        }

        [Fact]
        public async Task Dispose_Should_Dispose_A_Client_Whose_Async_Factory_Is_In_Progress()
        {
            using var cache = new FileProviderClientCache<DisposableClient>();
            var completion = new TaskCompletionSource<DisposableClient>(TaskCreationOptions.RunContinuationsAsynchronously);
            var first = cache.GetOrAddAsync("key", () => completion.Task);
            var second = cache.GetOrAddAsync("key", () => completion.Task);
            var client = new DisposableClient("client");

            cache.Dispose();
            completion.SetResult(client);

            await Assert.ThrowsAsync<ObjectDisposedException>(() => first);
            await Assert.ThrowsAsync<ObjectDisposedException>(() => second);
            Assert.Equal(1, client.DisposeCount);
        }

        [Fact]
        public async Task Dispose_Should_Dispose_A_Client_Whose_Sync_Factory_Is_In_Progress()
        {
            using var cache = new FileProviderClientCache<DisposableClient>();
            using var started = new ManualResetEventSlim();
            using var finish = new ManualResetEventSlim();
            var client = new DisposableClient("client");
            var request = Task.Run(() => cache.GetOrAdd("key", () =>
            {
                started.Set();
                if (!finish.Wait(TimeSpan.FromSeconds(10)))
                {
                    throw new TimeoutException("Factory was not released.");
                }
                return client;
            }));

            try
            {
                Assert.True(started.Wait(TimeSpan.FromSeconds(10)));
                cache.Dispose();
            }
            finally
            {
                finish.Set();
            }

            await Assert.ThrowsAsync<ObjectDisposedException>(() => request);
            Assert.Equal(1, client.DisposeCount);
        }

        [Fact]
        public async Task A_Failed_Async_Waiter_Should_Not_Remove_A_Successful_Retry()
        {
            await Task.Run(async () =>
            {
                using var cache = new FileProviderClientCache<DisposableClient>();
                var completion = new TaskCompletionSource<DisposableClient>();
                var first = cache.GetOrAddAsync("key", () => completion.Task);
                var second = cache.GetOrAddAsync("key", () => completion.Task);
                var retry = first.ContinueWith(
                    _ => cache.GetOrAddAsync("key", () => Task.FromResult(new DisposableClient("retry"))),
                    CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default).Unwrap();

                completion.SetException(new InvalidOperationException("boom"));

                await Assert.ThrowsAsync<InvalidOperationException>(() => first);
                await Assert.ThrowsAsync<InvalidOperationException>(() => second);
                var client = await retry;
                var cached = await cache.GetOrAddAsync("key", () => Task.FromResult(new DisposableClient("unexpected")));
                Assert.Same(client, cached);
            });
        }

        [Fact]
        public void Dispose_Should_Dispose_A_Client_Retired_While_Disposing()
        {
            using var cache = new DisposingOnRetireCache();
            var client = cache.GetOrAdd("key", () => new DisposableClient("client"));

            cache.SweepExpiredClients();

            Assert.Equal(1, client.DisposeCount);
        }

        [Fact]
        public void GetOrAdd_Should_Throw_After_Dispose()
        {
            var cache = new FileProviderClientCache<DisposableClient>();
            cache.Dispose();

            Assert.Throws<ObjectDisposedException>(
                () => cache.GetOrAdd("key", () => new DisposableClient("client")));
        }
    }
}
