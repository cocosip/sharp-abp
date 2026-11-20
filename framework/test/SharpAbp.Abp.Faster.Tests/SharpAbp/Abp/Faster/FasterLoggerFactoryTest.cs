using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace SharpAbp.Abp.Faster;

public class FasterLoggerFactoryTest
{
    [Fact]
    public async Task Descendant_Context_Can_Dispose_After_Initialization_Finishes()
    {
        var factory = new LifecycleFactory();
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        Task descendant = null;
        factory.OnCreate = () => descendant = Task.Run(async () =>
        {
            await release.Task;
            factory.Dispose();
        });
        factory.GetOrCreate<FasterTestEntry>("creating");
        release.SetResult();
        await descendant.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Equal(1, factory.Disposals);
    }

    [Fact]
    public void Initialization_Rejects_Disposal_Without_Poisoning_The_Factory()
    {
        var factory = new LifecycleFactory();
        factory.GetOrCreate<FasterTestEntry>("existing");
        Exception rejection = null;
        factory.OnCreate = () => rejection = Record.Exception(factory.Dispose);
        factory.GetOrCreate<FasterTestEntry>("creating");
        Assert.IsType<InvalidOperationException>(rejection);
        Assert.NotNull(factory.GetOrCreate<FasterTestEntry>("existing"));
        factory.Dispose();
        Assert.Equal(2, factory.Disposals);
    }

    [Fact]
    public void Dispose_Is_Terminal_And_Reports_All_Cleanup_Failures()
    {
        var factory = new LifecycleFactory();
        factory.GetOrCreate<FasterTestEntry>("first");
        factory.GetOrCreate<FasterTestEntry>("second");
        factory.ThrowOnDispose = true;
        var error = Assert.Throws<AggregateException>(() => factory.Dispose());
        Assert.Equal(2, error.InnerExceptions.Count);
        Assert.Equal(2, factory.Disposals);
        factory.Dispose();
        Assert.Equal(2, factory.Disposals);
        Assert.Throws<ObjectDisposedException>(() => factory.GetOrCreate<FasterTestEntry>("third"));
    }

    [Fact]
    public async Task Disposal_Waits_For_InFlight_Logger_And_Disposes_It_Once()
    {
        using var entered = new ManualResetEventSlim();
        using var release = new ManualResetEventSlim();
        var factory = new LifecycleFactory { Entered = entered, Release = release };
        var get = Task.Run(() => factory.GetOrCreate<FasterTestEntry>("pending"));
        Assert.True(entered.Wait(TimeSpan.FromSeconds(10)));
        var disposal = Task.Run(() => factory.Dispose());
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
        Assert.Equal(1, factory.Disposals);
        factory.Dispose();
        Assert.Equal(1, factory.Disposals);
        Assert.Throws<ObjectDisposedException>(() => factory.GetOrCreate<FasterTestEntry>("pending"));
    }

    [Fact]
    public void Failed_Initialization_Does_Not_Remove_A_Replacement_Entry()
    {
        using var factory = new ReplacedEntryFactory();

        Assert.Throws<InvalidOperationException>(() => factory.GetOrCreate<FasterTestEntry>("retry"));

        Assert.Same(factory.Replacement, factory.GetOrCreate<FasterTestEntry>("retry"));
    }

    private sealed class ReplacedEntryFactory : DefaultFasterLoggerFactory
    {
        public IFasterLogger<FasterTestEntry> Replacement { get; } = Mock.Of<IFasterLogger<FasterTestEntry>>();

        public ReplacedEntryFactory()
            : base(Microsoft.Extensions.Options.Options.Create(new AbpFasterOptions()), NullLogger<DefaultFasterLoggerFactory>.Instance,
                Mock.Of<IServiceProvider>())
        {
        }

        protected override IFasterLogger<T> CreateLogger<T>(string name)
        {
            // Model another caller replacing a failed lazy before this caller handles its failure.
            Loggers[new LoggerCacheKey(typeof(T), name)] = new Lazy<IFasterLogger>(() => Replacement);
            throw new InvalidOperationException("Initialization failed.");
        }
    }

    private sealed class LifecycleFactory : DefaultFasterLoggerFactory
    {
        public ManualResetEventSlim Entered { get; init; }
        public ManualResetEventSlim Release { get; init; }
        public bool ThrowOnDispose { get; set; }
        public Action OnCreate { get; set; }
        public int Disposals;

        public LifecycleFactory()
            : base(Microsoft.Extensions.Options.Options.Create(new AbpFasterOptions()), NullLogger<DefaultFasterLoggerFactory>.Instance,
                Mock.Of<IServiceProvider>())
        {
        }

        protected override IFasterLogger<T> CreateLogger<T>(string name)
        {
            OnCreate?.Invoke();
            Entered?.Set();
            if (Release != null) Assert.True(Release.Wait(TimeSpan.FromSeconds(10)));
            var logger = new Mock<IFasterLogger<T>>();
            logger.Setup(x => x.Dispose()).Callback(() =>
            {
                Interlocked.Increment(ref Disposals);
                if (ThrowOnDispose) throw new InvalidOperationException("cleanup failed");
            });
            return logger.Object;
        }
    }
}
