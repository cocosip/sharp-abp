﻿﻿﻿using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.MinId
{
    /// <summary>
    /// Defines the contract for segment ID management service.
    /// This service is responsible for allocating and managing ID segments
    /// for different business types in a distributed environment.
    /// </summary>
    public interface ISegmentIdService
    {
        /// <summary>
        /// Retrieves the next available segment for the specified business type.
        /// This method allocates a new segment with a range of IDs that can be
        /// used for unique identifier generation.
        /// </summary>
        /// <param name="bizType">The business type identifier for which to allocate a segment.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task containing the newly allocated segment with ID range information.</returns>
        Task<SegmentId> GetNextSegmentIdAsync(string bizType, CancellationToken cancellationToken = default);
    }
}