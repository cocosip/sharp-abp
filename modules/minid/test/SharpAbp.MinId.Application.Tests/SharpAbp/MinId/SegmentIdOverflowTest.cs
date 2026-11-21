using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.MinId;

public class SegmentIdOverflowTest
{
    [Theory]
    [InlineData(long.MinValue, 3, 2, long.MinValue + 1)]
    [InlineData(-10, 3, 2, -7L)]
    [InlineData(-3, 5, 4, -1L)]
    public void Signed_Alignment_Uses_Nonnegative_Modulo(long previousId, int delta, int remainder, long expected)
    {
        var segment = Create(previousId, delta, remainder);
        Assert.Equal(expected, segment.NextId().Item2);
        Assert.Equal(expected + delta, segment.NextId().Item2);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(2, 2)]
    [InlineData(2, -1)]
    public void Invalid_Stored_Segment_Configuration_Fails_Explicitly(int delta, int remainder)
    {
        var segment = Create(0, delta, remainder);
        Assert.Throws<System.InvalidOperationException>(() => segment.NextId());
    }

    [Theory]
    [InlineData(1, 0, 1)]
    [InlineData(2, 1, 2)]
    [InlineData(3, 1, 1)]
    public void Terminal_Aligned_Id_Is_Returned_Once_Then_Segment_Is_Exhausted(int delta, int remainder, int distance)
    {
        var segment = Create(long.MaxValue - distance, delta, remainder);

        Assert.Equal((ResultCodeConsts.Loading, long.MaxValue), segment.NextId());
        Assert.Equal((ResultCodeConsts.Over, long.MaxValue), segment.NextId());
        Assert.Equal((ResultCodeConsts.Over, long.MaxValue), segment.NextId());
        Assert.Equal(long.MaxValue, segment.CurrentId);
        Assert.False(segment.IsUseful());
    }

    [Theory]
    [InlineData(2, 0)]
    [InlineData(3, 2)]
    public void Terminal_Range_Without_Another_Aligned_Id_Does_Not_Wrap(int delta, int remainder)
    {
        var segment = Create(long.MaxValue - 1, delta, remainder);

        var (code, id) = segment.NextId();

        Assert.Equal(ResultCodeConsts.Over, code);
        Assert.True(id >= 0);
        Assert.True(segment.CurrentId >= 0);
        Assert.False(segment.IsUseful());
    }

    [Fact]
    public void Concurrent_Terminal_Allocation_Returns_Each_Available_Id_Once()
    {
        var segment = Create(long.MaxValue - 10, 1, 0);
        var results = new ConcurrentBag<(int Code, long Id)>();

        Parallel.For(0, 32, _ => results.Add(segment.NextId()));

        var ids = results.Where(result => result.Code != ResultCodeConsts.Over).Select(result => result.Id).ToArray();
        Assert.Equal(10, ids.Length);
        Assert.Equal(10, ids.Distinct().Count());
        Assert.Equal(long.MaxValue - 9, ids.Min());
        Assert.Equal(long.MaxValue, ids.Max());
        Assert.All(results, result => Assert.True(result.Id >= 0));
        Assert.False(segment.IsUseful());
    }

    private static SegmentId Create(long currentId, int delta, int remainder) => new()
    {
        CurrentId = currentId,
        MaxId = long.MaxValue,
        LoadingId = long.MaxValue,
        Delta = delta,
        Remainder = remainder
    };
}
