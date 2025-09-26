﻿using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.MinId
{
    /// <summary>
    /// Application service for managing segment IDs.
    /// This service acts as a bridge between the presentation layer and the domain layer,
    /// providing operations for segment ID allocation and management.
    /// </summary>
    public class SegmentIdAppService : MinIdAppService, ISegmentIdAppService
    {
        /// <summary>
        /// Gets the segment ID service for domain operations.
        /// </summary>
        protected ISegmentIdService SegmentIdService { get; }
        
        /// <summary>
        /// Initializes a new instance of the SegmentIdAppService class.
        /// </summary>
        /// <param name="segmentIdService">The segment ID service for domain operations.</param>
        public SegmentIdAppService(ISegmentIdService segmentIdService)
        {
            SegmentIdService = segmentIdService;
        }

        /// <summary>
        /// Retrieves the next available segment for the specified business type.
        /// This method allocates a new segment with a range of IDs that can be
        /// used for unique identifier generation.
        /// </summary>
        /// <param name="bizType">The business type identifier for which to allocate a segment.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task containing the newly allocated segment with ID range information.</returns>
        public virtual async Task<SegmentIdDto> GetNextSegmentIdAsync(string bizType, CancellationToken cancellationToken = default)
        {
            var segmentId = await SegmentIdService.GetNextSegmentIdAsync(bizType, cancellationToken);
            return ObjectMapper.Map<SegmentId, SegmentIdDto>(segmentId);
        }
    }
}