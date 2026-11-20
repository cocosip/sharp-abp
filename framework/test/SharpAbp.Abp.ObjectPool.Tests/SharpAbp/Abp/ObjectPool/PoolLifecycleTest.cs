using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.ObjectPool;
using Xunit;

namespace SharpAbp.Abp.ObjectPool;

public class PoolLifecycleTest
{
    [Fact]
    public async Task Descendant_Context_Can_Remove_Pools_After_Initialization_Finishes()
    {
        using var owner = new PoolOrchestrator();
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        Task descendant = null;
        owner.GetPool<Item>("creating", () =>
        {
            descendant = Task.Run(async () =>
            {
                await release.Task;
                owner.RemoveAll();
            });
            return new Policy();
        });
        release.SetResult();
        await descendant.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.NotNull(owner.GetPool("reused", new Policy()));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void Initialization_Rejects_Lifecycle_Mutation_Without_Changing_Owner_State(int operation)
    {
        var owner = new PoolOrchestrator();
        var retained = new Item();
        owner.GetPool("existing", new Policy()).Return(retained);
        Exception rejection = null;
        owner.GetPool<Item>("creating", () =>
        {
            rejection = Record.Exception(() =>
            {
                if (operation == 0) owner.Dispose();
                else if (operation == 1) owner.RemoveAll();
                else owner.RemovePool<Item>("existing");
            });
            return new Policy();
        });
        Assert.IsType<InvalidOperationException>(rejection);
        Assert.Equal(0, retained.Disposals);
        Assert.NotNull(owner.GetPool("still-usable", new Policy()));
        owner.Dispose();
        Assert.Equal(1, retained.Disposals);
    }

    [Fact]
    public async Task Async_Cleanup_Can_Access_The_Reusable_Owner_Without_Deadlock()
    {
        var owner = new PoolOrchestrator();
        var item = new CallbackItem(async () =>
        {
            await Task.Yield();
            await Task.Run(() =>
            {
                owner.GetPool("from-cleanup", new Policy());
                owner.RemovePool<Item>("from-cleanup");
            }).WaitAsync(TimeSpan.FromSeconds(2));
        });
        var pool = owner.GetAsyncObjectPool("cleanup", new CallbackPolicy());
        pool.Return(item);
        await Task.Run(() => owner.RemoveAll()).WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Equal(1, item.Disposals);
        owner.Dispose();
    }

    [Fact]
    public void Dispose_Is_Terminal_But_RemoveAll_Allows_Reuse()
    {
        var owner = new PoolOrchestrator();
        var first = owner.GetPool("items", new Policy());
        Assert.Equal(1, owner.RemoveAll());
        Assert.NotSame(first, owner.GetPool("items", new Policy()));
        owner.Dispose();
        owner.Dispose();
        Assert.Throws<ObjectDisposedException>(() => owner.GetPool("items", new Policy()));
        Assert.Throws<ObjectDisposedException>(() => owner.GetAsyncObjectPool("items", new AsyncPolicy()));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Removal_Waits_For_InFlight_Pool_Creation(bool dispose)
    {
        var owner = new PoolOrchestrator();
        using var entered = new ManualResetEventSlim();
        using var release = new ManualResetEventSlim();
        var get = Task.Run(() => owner.GetPool<Item>("pending", () =>
        {
            entered.Set();
            Assert.True(release.Wait(TimeSpan.FromSeconds(10)));
            return new Policy();
        }));
        Assert.True(entered.Wait(TimeSpan.FromSeconds(10)));
        var removal = Task.Run(() => { if (dispose) owner.Dispose(); else owner.RemovePool<Item>("pending"); });
        try
        {
            Assert.NotSame(removal, await Task.WhenAny(removal, Task.Delay(100)));
        }
        finally
        {
            release.Set();
        }
        var pool = await get.WaitAsync(TimeSpan.FromSeconds(10));
        await removal.WaitAsync(TimeSpan.FromSeconds(10));
        Assert.Throws<ObjectDisposedException>(() => pool.Get());
        owner.Dispose();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Pool_Disposal_Attempts_Every_Item_And_Reports_Failures(bool async)
    {
        var first = new Item { ThrowOnDispose = true };
        var second = new Item();
        if (async)
        {
            var pool = new AsyncObjectPool<Item>(new AsyncPolicy(), 2);
            pool.Return(first);
            pool.Return(second);
            Assert.Throws<AggregateException>(() => pool.Dispose());
            pool.Dispose();
        }
        else
        {
            var pool = new BoundedObjectPool<Item>(new Policy(), 2);
            pool.Return(first);
            pool.Return(second);
            Assert.Throws<AggregateException>(() => pool.Dispose());
            pool.Dispose();
        }
        Assert.Equal(1, first.Disposals);
        Assert.Equal(1, second.Disposals);
    }

    [Fact]
    public void Orchestrator_Disposal_Attempts_Every_Pool_And_Reports_Failures()
    {
        var owner = new PoolOrchestrator();
        var first = new Item { ThrowOnDispose = true };
        var second = new Item { ThrowOnDispose = true };
        owner.GetPool("first", new Policy()).Return(first);
        owner.GetPool("second", new Policy()).Return(second);
        var error = Assert.Throws<AggregateException>(() => owner.Dispose());
        Assert.Equal(2, error.Flatten().InnerExceptions.Count);
        Assert.Equal(1, first.Disposals);
        Assert.Equal(1, second.Disposals);
        owner.Dispose();
        Assert.Throws<ObjectDisposedException>(() => owner.GetPool("third", new Policy()));
    }

    [Fact]
    public async Task Pending_Async_Item_Is_Disposed_If_Pool_Shuts_Down()
    {
        var source = new TaskCompletionSource<Item>(TaskCreationOptions.RunContinuationsAsynchronously);
        var policy = new AsyncPolicy { Pending = source.Task };
        var pool = new AsyncObjectPool<Item>(policy, 1);
        var pending = pool.GetAsync().AsTask();
        pool.Dispose();
        var item = new Item();
        source.SetResult(item);
        await Assert.ThrowsAsync<ObjectDisposedException>(() => pending);
        Assert.Equal(1, item.Disposals);
    }

    [Fact]
    public void Removal_Reports_Failures_From_Both_Sync_And_Async_Pools_And_Allows_Reuse()
    {
        using var owner = new PoolOrchestrator();
        var first = new Item { ThrowOnDispose = true };
        var second = new Item { ThrowOnDispose = true };
        owner.GetPool("shared", new Policy()).Return(first);
        owner.GetAsyncObjectPool("shared", new AsyncPolicy()).Return(second);
        var error = Assert.Throws<AggregateException>(() => owner.RemovePool<Item>("shared"));
        Assert.Equal(2, error.Flatten().InnerExceptions.Count);
        Assert.Equal(1, first.Disposals);
        Assert.Equal(1, second.Disposals);
        Assert.Equal(0, owner.RemoveAll());
        Assert.NotNull(owner.GetPool("shared", new Policy()));
    }

    [Fact]
    public async Task Pending_AsyncOnly_Item_Uses_Async_Disposal_After_Shutdown()
    {
        var source = new TaskCompletionSource<AsyncOnlyItem>(TaskCreationOptions.RunContinuationsAsynchronously);
        var pool = new AsyncObjectPool<AsyncOnlyItem>(new AsyncOnlyPolicy(source.Task), 1);
        var pending = pool.GetAsync().AsTask();
        pool.Dispose();
        var item = new AsyncOnlyItem();
        source.SetResult(item);
        await Assert.ThrowsAsync<ObjectDisposedException>(() => pending);
        Assert.Equal(1, item.Disposals);
    }

    private sealed class Item : IDisposable
    {
        public bool ThrowOnDispose { get; init; }
        public int Disposals { get; private set; }
        public void Dispose()
        {
            Disposals++;
            if (ThrowOnDispose) throw new InvalidOperationException("cleanup failed");
        }
    }

    private sealed class Policy : IPooledObjectPolicy<Item>
    {
        public Item Create() => new();
        public bool Return(Item item) => true;
    }

    private sealed class AsyncPolicy : IAsyncObjectPoolPolicy<Item>
    {
        public Task<Item> Pending { get; init; }
        public ValueTask<Item> CreateAsync() => Pending == null ? new(new Item()) : new(Pending);
        public bool Return(Item item) => true;
    }

    private sealed class AsyncOnlyItem : IAsyncDisposable
    {
        public int Disposals { get; private set; }
        public async ValueTask DisposeAsync()
        {
            await Task.Yield();
            Disposals++;
        }
    }

    private sealed class AsyncOnlyPolicy(Task<AsyncOnlyItem> pending) : IAsyncObjectPoolPolicy<AsyncOnlyItem>
    {
        public ValueTask<AsyncOnlyItem> CreateAsync() => new(pending);
        public bool Return(AsyncOnlyItem item) => true;
    }

    private sealed class CallbackItem(Func<Task> callback) : IAsyncDisposable
    {
        public int Disposals { get; private set; }
        public async ValueTask DisposeAsync()
        {
            Disposals++;
            await callback();
        }
    }

    private sealed class CallbackPolicy : IAsyncObjectPoolPolicy<CallbackItem>
    {
        public ValueTask<CallbackItem> CreateAsync() => throw new NotSupportedException();
        public bool Return(CallbackItem item) => true;
    }
}
