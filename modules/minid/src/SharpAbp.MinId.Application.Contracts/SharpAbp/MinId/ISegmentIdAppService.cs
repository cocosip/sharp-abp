using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace SharpAbp.MinId
{
    public interface ISegmentIdAppService : IApplicationService
    {
        /// <summary>
        /// Get next segmentId by bizType
        /// </summary>
        /// <param name="bizType"></param>
        /// <returns></returns>
        Task<SegmentIdDto> GetNextSegmentIdAsync(string bizType);
    }
}
