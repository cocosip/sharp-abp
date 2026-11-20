using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace SharpAbp.MinId;

public class MinIdBatchLimitTest
{
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1001)]
    public async Task Both_Generator_Boundaries_Reject_Invalid_Batches_Before_Allocating(int batchSize)
    {
        var factory = Substitute.For<IMinIdGeneratorFactory>();
        var application = new MinIdGeneratorAppService(factory);
        var segments = Substitute.For<ISegmentIdService>();
        var generator = new DefaultMinIdGenerator("orders", Options.Create(new MinIdOptions()), segments);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => application.NextIdAsync("orders", batchSize));
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => generator.NextIdAsync(batchSize));
        await factory.DidNotReceiveWithAnyArgs().GetAsync(default);
        await segments.DidNotReceiveWithAnyArgs().GetNextSegmentIdAsync(default);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(1000)]
    public async Task Supported_Batches_Generate_The_Requested_Number_Of_Ids(int batchSize)
    {
        var segments = Substitute.For<ISegmentIdService>();
        segments.GetNextSegmentIdAsync("orders").Returns(new SegmentId
        {
            CurrentId = 0, MaxId = 2000, Delta = 1, LoadingId = 1800
        });
        var generator = new DefaultMinIdGenerator("orders", Options.Create(new MinIdOptions()), segments);

        var ids = await generator.NextIdAsync(batchSize);

        Assert.Equal(batchSize, ids.Count);
        Assert.Equal(1, ids[0]);
        Assert.Equal(batchSize, ids[^1]);
    }
}
