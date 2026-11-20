using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;
using System;
using Volo.Abp;

namespace SharpAbp.MinId;

public class DefaultMinIdGeneratorTest
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Segment_Cancellation_Propagates_Through_The_Generator_And_Controller(bool preload)
    {
        var service = Substitute.For<ISegmentIdService>();
        var cancelled = Task.FromException<SegmentId>(new OperationCanceledException());
        if (preload)
        {
            service.GetNextSegmentIdAsync("orders", Arg.Any<CancellationToken>()).Returns(
                Task.FromResult(new SegmentId { CurrentId = 0, MaxId = 10, Delta = 1, LoadingId = 1 }), cancelled);
        }
        else
        {
            service.GetNextSegmentIdAsync("orders", Arg.Any<CancellationToken>()).Returns(cancelled);
        }
        var generator = new DefaultMinIdGenerator("orders", Options.Create(new MinIdOptions()), service);
        var factory = Substitute.For<IMinIdGeneratorFactory>();
        factory.GetAsync("orders").Returns(generator);
        var controller = new IdController(Substitute.For<IMinIdTokenValidator>(), new MinIdGeneratorAppService(factory),
            Substitute.For<ISegmentIdAppService>());

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => controller.NextIdAsync("orders", "Bearer exampletoken"));
    }

    [Fact]
    public async Task Segment_Load_Failure_Does_Not_Copy_Backend_Details_Into_Public_Message()
    {
        const string detail = "private-backend-details";
        var service = Substitute.For<ISegmentIdService>();
        service.GetNextSegmentIdAsync("orders", Arg.Any<CancellationToken>())
            .Returns<SegmentId>(_ => throw new InvalidOperationException(detail));
        var generator = new DefaultMinIdGenerator("orders", Options.Create(new MinIdOptions()), service);

        var error = await Assert.ThrowsAsync<AbpException>(() => generator.NextIdAsync());

        Assert.DoesNotContain(detail, error.Message);
        Assert.Contains("orders", error.Message);
        Assert.IsType<InvalidOperationException>(error.InnerException);
    }

    [Fact]
    public async Task Concurrent_Current_Loads_Preserve_The_First_Usable_Segment()
    {
        var pending = new TaskCompletionSource<SegmentId>(TaskCreationOptions.RunContinuationsAsynchronously);
        var service = Substitute.For<ISegmentIdService>();
        service.GetNextSegmentIdAsync("orders", Arg.Any<CancellationToken>()).Returns(
            pending.Task,
            Task.FromResult(new SegmentId { CurrentId = 100, MaxId = 200, Delta = 1, LoadingId = 180 }));
        var generator = new DefaultMinIdGenerator("orders", Options.Create(new MinIdOptions()), service);

        var first = generator.LoadCurrentAsync();
        var second = generator.LoadCurrentAsync();
        pending.SetResult(new SegmentId { CurrentId = 0, MaxId = 100, Delta = 1, LoadingId = 80 });
        await Task.WhenAll(first, second);

        Assert.Equal(1, await generator.NextIdAsync());
    }
}
