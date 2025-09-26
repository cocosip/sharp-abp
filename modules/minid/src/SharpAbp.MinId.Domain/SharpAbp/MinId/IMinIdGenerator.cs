﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpAbp.MinId
{
    /// <summary>
    /// Provides functionality for generating minimum identifiers using segment-based allocation.
    /// This generator supports both single ID generation and batch operations with automatic segment management.
    /// </summary>
    public interface IMinIdGenerator
    {
        /// <summary>
        /// Loads the current segment for ID generation.
        /// If the current segment is exhausted or invalid, this method will acquire a new segment.
        /// </summary>
        /// <returns>A task representing the asynchronous load operation.</returns>
        Task LoadCurrentAsync();

        /// <summary>
        /// Pre-loads the next segment for ID generation to ensure continuous ID allocation.
        /// This method is typically called when the current segment is nearing exhaustion.
        /// </summary>
        /// <returns>A task representing the asynchronous pre-load operation.</returns>
        Task LoadNextAsync();

        /// <summary>
        /// Generates the next unique identifier from the current segment.
        /// Automatically handles segment transitions and loading when necessary.
        /// </summary>
        /// <returns>A task containing the next unique identifier.</returns>
        Task<long> NextIdAsync();

        /// <summary>
        /// Generates a batch of unique identifiers with the specified size.
        /// This method efficiently generates multiple IDs in sequence.
        /// </summary>
        /// <param name="batchSize">The number of unique identifiers to generate.</param>
        /// <returns>A task containing a list of unique identifiers.</returns>
        Task<List<long>> NextIdAsync(int batchSize);
    }
}
