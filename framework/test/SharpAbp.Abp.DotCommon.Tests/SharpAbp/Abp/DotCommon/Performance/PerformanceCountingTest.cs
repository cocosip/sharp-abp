using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using Xunit;

namespace SharpAbp.Abp.DotCommon.Performance;

public class PerformanceCountingTest
{
    [Fact]
    public void Calculation_Should_Keep_Total_And_Interval_Metrics_Separate()
    {
        using var services = new ServiceCollection().BuildServiceProvider();
        var count = new CountInfo(NullLogger<CountInfo>.Instance, services, "api", "read",
            new PerformanceConfiguration { AutoLogging = false }, 2, 8);
        count.IncrementTotalCount(4);

        count.Calculate();

        Assert.Equal(3, count.TotalCount);
        Assert.Equal(3, count.Throughput);
        Assert.Equal(3, count.AverageThroughput);
        Assert.Equal(4, count.RT);
        Assert.Equal(4, count.AverageRT);
        count.Calculate();
        Assert.Equal(0, count.Throughput);
        Assert.Equal(3, count.AverageThroughput);
        Assert.Equal(4, count.RT);
        count.IncrementTotalCount(6);
        count.Calculate();
        var info = count.GetCurrentPerformanceInfo();
        Assert.Equal(4, info.TotalCount);
        Assert.Equal(1, info.Throughput);
        Assert.Equal(2, info.AverageThroughput);
        Assert.Equal(6, info.RT);
        Assert.Equal(4.5, info.AverageRT);
    }

    [Fact]
    public void Empty_Interval_And_Absolute_Count_Updates_Should_Not_Divide_By_Zero()
    {
        using var services = new ServiceCollection().BuildServiceProvider();
        var count = new CountInfo(NullLogger<CountInfo>.Instance, services, "api", "read",
            new PerformanceConfiguration { AutoLogging = false }, 0, 0);
        count.Calculate();
        Assert.Equal(0, count.RT);
        Assert.Equal(0, count.AverageRT);
        Assert.Equal(0, count.AverageThroughput);

        count.UpdateTotalCount(4, 20);
        count.Calculate();

        Assert.Equal(4, count.TotalCount);
        Assert.Equal(4, count.Throughput);
        Assert.Equal(5, count.RT);
        Assert.Equal(5, count.AverageRT);
    }

    [Fact]
    public void Calculation_Should_Publish_Snapshot_Through_Registered_Handler_And_Log_Context()
    {
        var handler = new RecordingHandler();
        using var services = new ServiceCollection()
            .AddSingleton(handler)
            .AddSingleton<ContextText>()
            .BuildServiceProvider();
        var configuration = new PerformanceConfiguration { AutoLogging = true };
        configuration.LogContextTexts.Add(typeof(ContextText));
        configuration.PerformanceInfoHandlers.Add(typeof(RecordingHandler));
        var count = new CountInfo(NullLogger<CountInfo>.Instance, services, "api", "read", configuration, 1, 7);

        count.Calculate();

        var snapshot = Assert.Single(handler.Snapshots);
        Assert.Equal("api", snapshot.Name);
        Assert.Equal("read", snapshot.Key);
        Assert.Equal(1, snapshot.Info.TotalCount);
        Assert.Equal(7, snapshot.Info.RT);
    }

    [Fact]
    public void Handler_Failure_Should_Not_Prevent_Future_Metric_Updates()
    {
        using var services = new ServiceCollection().AddSingleton<ThrowingHandler>().BuildServiceProvider();
        var configuration = new PerformanceConfiguration { AutoLogging = true };
        configuration.PerformanceInfoHandlers.Add(typeof(ThrowingHandler));
        var count = new CountInfo(NullLogger<CountInfo>.Instance, services, "api", "read", configuration, 1, 2);

        count.Calculate();
        count.IncrementTotalCount(4);
        count.Calculate();

        Assert.Equal(2, count.TotalCount);
        Assert.Equal(4, count.RT);
        Assert.Equal(3, count.AverageRT);
    }

    public sealed class RecordingHandler : IPerformanceInfoHandlerService
    {
        public List<(string Name, string Key, PerformanceInfo Info)> Snapshots { get; } = [];
        public void Handle(string name, string key, PerformanceInfo info) => Snapshots.Add((name, key, info));
    }

    public sealed class ContextText : ILogContextTextService
    {
        public string GetLogContextText(string name, string key) => "request=42";
    }

    public sealed class ThrowingHandler : IPerformanceInfoHandlerService
    {
        public void Handle(string name, string key, PerformanceInfo info) => throw new InvalidOperationException("Sink unavailable");
    }
}

public class PerformanceServiceFactoryTest : AbpDotCommonTestBase
{
    [Fact]
    public void Factory_Should_Reuse_Named_Service_And_Track_Independent_Keys()
    {
        var factory = GetRequiredService<IPerformanceServiceFactory>();
        try
        {
            var first = factory.GetOrCreate("orders");
            Assert.Same(first, factory.GetOrCreate("orders"));
            Assert.NotSame(first, factory.GetOrCreate("payments"));
            Assert.Null(first.GetKeyPerformanceInfo("missing"));
            first.IncrementKeyCount("read", 3);
            first.IncrementKeyCount("read", 5);
            first.UpdateKeyCount("write", 10, 20);
            first.UpdateKeyCount("write", 12, 8);

            Assert.Equal(2, first.GetKeyPerformanceInfo("read").TotalCount);
            Assert.Equal(12, first.GetKeyPerformanceInfo("write").TotalCount);
            Assert.Null(factory.GetOrCreate("payments").GetKeyPerformanceInfo("read"));
        }
        finally
        {
            factory.StopAll();
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Factory_Should_Reject_Empty_Service_Name(string name)
    {
        Assert.ThrowsAny<ArgumentException>(() => GetRequiredService<IPerformanceServiceFactory>().GetOrCreate(name));
    }
}
