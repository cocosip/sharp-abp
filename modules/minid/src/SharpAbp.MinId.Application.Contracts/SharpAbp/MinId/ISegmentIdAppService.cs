﻿using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace SharpAbp.MinId
{
    public interface ISegmentIdAppService : IApplicationService
    {
        /// <summary>
        /// Retrieves the next available segment for the specified business type.
        /// This method allocates a new segment with a range of IDs that can be
        /// used for unique identifier generation.
        /// </summary>
        /// <param name="bizType">The business type identifier for which to allocate a segment.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task containing the newly allocated segment with ID range information.</returns>
        Task<SegmentIdDto> GetNextSegmentIdAsync(string bizType, CancellationToken cancellationToken = default);
    }
}