using System.Threading.Tasks;

namespace SharpAbp.MinId
{
    public interface ISegmentIdService
    {
        /// <summary>
        /// Get next segmentId by bizType
        /// </summary>
        /// <param name="bizType"></param>
        /// <returns></returns>
        Task<SegmentId> GetNextSegmentIdAsync(string bizType);
    }
}
