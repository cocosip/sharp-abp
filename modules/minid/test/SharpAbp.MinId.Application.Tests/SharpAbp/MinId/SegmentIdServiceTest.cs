using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.MinId
{
    public class SegmentIdServiceTest : MinIdApplicationTestBase
    {
        private readonly ISegmentIdService _segmentIdService;
        public SegmentIdServiceTest()
        {
            _segmentIdService = GetRequiredService<ISegmentIdService>();
        }

        [Fact]
        public virtual async Task GetNextSegmentId_Test_Async()
        {
            var segmentId1 = await _segmentIdService.GetNextSegmentIdAsync("default");
            var segmentId2 = await _segmentIdService.GetNextSegmentIdAsync("default");

            Assert.NotEqual(segmentId1.MaxId, segmentId2.MaxId);
            Assert.True(segmentId1.MaxId < segmentId2.MaxId);

            Assert.Equal(segmentId1.Delta, segmentId2.Delta);
            Assert.Equal(segmentId1.Remainder, segmentId2.Remainder);
        }


    }
}
