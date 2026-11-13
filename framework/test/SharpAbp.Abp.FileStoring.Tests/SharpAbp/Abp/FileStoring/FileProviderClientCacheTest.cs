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

            public DisposableClient(string name)
            {
                Name = name;
            }

            public void Dispose()
            {
                IsDisposed = true;
            }
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
        public void GetOrAdd_Should_Throw_After_Dispose()
        {
            var cache = new FileProviderClientCache<DisposableClient>();
            cache.Dispose();

            Assert.Throws<ObjectDisposedException>(
                () => cache.GetOrAdd("key", () => new DisposableClient("client")));
        }
    }
}
