using System.Threading.Tasks;

namespace SharpAbp.MinId
{
    public class SegmentIdAppService : MinIdAppService, ISegmentIdAppService
    {
        protected ISegmentIdService SegmentIdService { get; }
        public SegmentIdAppService(ISegmentIdService segmentIdService)
        {
            SegmentIdService = segmentIdService;
        }

        /// <summary>
        /// Get next segmentId by bizType
        /// </summary>
        /// <param name="bizType"></param>
        /// <returns></returns>
        public virtual async Task<SegmentIdDto> GetNextSegmentIdAsync(string bizType)
        {
            var segmentId = await SegmentIdService.GetNextSegmentIdAsync(bizType);
            return ObjectMapper.Map<SegmentId, SegmentIdDto>(segmentId);
        }
    }
}
