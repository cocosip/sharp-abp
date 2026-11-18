using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.MinId
{
    public class SegmentIdServiceTest : MinIdApplicationTestBase
    {
        private readonly ISegmentIdService _segmentIdService;
        private readonly IMinIdInfoRepository _minIdInfoRepository;

        public SegmentIdServiceTest()
        {
            _segmentIdService = GetRequiredService<ISegmentIdService>();
            _minIdInfoRepository = GetRequiredService<IMinIdInfoRepository>();
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

        [Fact]
        public virtual async Task Update_Should_Refresh_ConcurrencyStamp()
        {
            var minIdInfo = await _minIdInfoRepository.FindByBizTypeAsync("default");
            Assert.NotNull(minIdInfo);

            var originalStamp = minIdInfo.ConcurrencyStamp;

            minIdInfo.UpdateMaxId(minIdInfo.MaxId + minIdInfo.Step);
            await _minIdInfoRepository.UpdateAsync(minIdInfo);

            var updated = await _minIdInfoRepository.FindByBizTypeAsync("default");
            Assert.NotNull(updated);
            Assert.NotEqual(originalStamp, updated.ConcurrencyStamp);
        }
    }
}
