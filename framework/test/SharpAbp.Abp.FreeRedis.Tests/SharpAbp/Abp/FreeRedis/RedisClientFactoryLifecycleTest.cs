using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FreeRedis;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace SharpAbp.Abp.FreeRedis;

public class RedisClientFactoryLifecycleTest
{
    [Fact]
    public async Task Descendant_Context_Can_Dispose_After_Initialization_Finishes()
    {
        DefaultRedisClientFactory factory = null;
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        Task descendant = null;
        factory = CreateFactory(() =>
        {
            descendant = Task.Run(async () =>
            {
                await release.Task;
                factory.Dispose();
            });
            return null;
        });
        factory.Get();
        release.SetResult();
        await descendant.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Throws<ObjectDisposedException>(() => factory.Get());
    }

    [Fact]
    public void Initialization_Rejects_Disposal_Without_Poisoning_The_Factory()
    {
        DefaultRedisClientFactory factory = null;
        Exception rejection = null;
        factory = CreateFactory(() =>
        {
            rejection = Record.Exception(factory.Dispose);
            return new RedisClient("127.0.0.1:1");
        });
        var client = factory.Get();
        Assert.IsType<InvalidOperationException>(rejection);
        Assert.Same(client, factory.Get());
        factory.Dispose();
        Assert.Throws<ObjectDisposedException>(() => factory.Get());
    }

    [Fact]
    public async Task Concurrent_First_Get_Creates_One_Shared_Client()
    {
        using var entered = new ManualResetEventSlim();
        using var release = new ManualResetEventSlim();
        using var ready = new CountdownEvent(8);
        using var start = new ManualResetEventSlim();
        var creations = 0;
        var factory = CreateFactory(() =>
        {
            Interlocked.Increment(ref creations);
            entered.Set();
            Assert.True(release.Wait(TimeSpan.FromSeconds(10)));
            return new RedisClient("127.0.0.1:1");
        });
        var gets = Enumerable.Range(0, 8).Select(_ => Task.Factory.StartNew(() =>
        {
            ready.Signal();
            Assert.True(start.Wait(TimeSpan.FromSeconds(10)));
            return factory.Get();
        }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default)).ToArray();
        try
        {
            Assert.True(ready.Wait(TimeSpan.FromSeconds(10)));
            start.Set();
            Assert.True(entered.Wait(TimeSpan.FromSeconds(10)));
            await Task.Delay(100);
        }
        finally
        {
            start.Set();
            release.Set();
        }
        var clients = await Task.WhenAll(gets).WaitAsync(TimeSpan.FromSeconds(10));
        Assert.Equal(1, creations);
        Assert.All(clients, client => Assert.Same(clients[0], client));
        (factory as IDisposable)?.Dispose();
    }

    [Fact]
    public void Dispose_Is_Terminal_And_Failed_Initialization_Can_Retry()
    {
        var attempts = 0;
        var factory = CreateFactory(() => ++attempts == 1
            ? throw new InvalidOperationException("transient initialization")
            : new RedisClient("127.0.0.1:1"));
        Assert.Throws<InvalidOperationException>(() => factory.Get());
        Assert.Same(factory.Get(), factory.Get());
        Assert.Equal(2, attempts);
        var disposable = Assert.IsAssignableFrom<IDisposable>(factory);
        disposable.Dispose();
        disposable.Dispose();
        Assert.Throws<ObjectDisposedException>(() => factory.Get());
        Assert.Throws<ObjectDisposedException>(() => factory.GetAll());
    }

    [Fact]
    public async Task Disposal_Waits_For_InFlight_Client_Creation()
    {
        using var entered = new ManualResetEventSlim();
        using var release = new ManualResetEventSlim();
        var factory = CreateFactory(() =>
        {
            entered.Set();
            Assert.True(release.Wait(TimeSpan.FromSeconds(10)));
            return new RedisClient("127.0.0.1:1");
        });
        var disposable = Assert.IsAssignableFrom<IDisposable>(factory);
        var get = Task.Run(() => factory.Get());
        Assert.True(entered.Wait(TimeSpan.FromSeconds(10)));
        var disposal = Task.Run(() => disposable.Dispose());
        try
        {
            Assert.NotSame(disposal, await Task.WhenAny(disposal, Task.Delay(100)));
        }
        finally
        {
            release.Set();
        }
        await get.WaitAsync(TimeSpan.FromSeconds(10));
        await disposal.WaitAsync(TimeSpan.FromSeconds(10));
        Assert.Throws<ObjectDisposedException>(() => factory.Get());
    }

    private static DefaultRedisClientFactory CreateFactory(Func<RedisClient> create)
    {
        var provider = new Mock<IRedisConfigurationProvider>();
        provider.Setup(x => x.Get(It.IsAny<string>())).Returns(new FreeRedisConfiguration());
        var builder = new Mock<IRedisClientBuilder>();
        builder.Setup(x => x.CreateClient(It.IsAny<FreeRedisConfiguration>())).Returns(create);
        return new DefaultRedisClientFactory(NullLogger<DefaultRedisClientFactory>.Instance, provider.Object, builder.Object);
    }
}
