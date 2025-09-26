﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace SharpAbp.MinId
{
    /// <summary>
    /// Application service interface for generating unique IDs using the MinId algorithm.
    /// Provides methods to generate single or batch IDs for a specified business type.
    /// </summary>
    public interface IMinIdGeneratorAppService : IApplicationService
    {
        /// <summary>
        /// Generates the next unique ID for the specified business type.
        /// </summary>
        /// <param name="bizType">The business type identifier for which to generate an ID.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the generated unique ID.</returns>
        Task<long> NextIdAsync(string bizType);

        /// <summary>
        /// Generates a batch of unique IDs for the specified business type.
        /// </summary>
        /// <param name="bizType">The business type identifier for which to generate IDs.</param>
        /// <param name="batchSize">The number of IDs to generate in the batch. Must be greater than 0.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of generated unique IDs.</returns>
        Task<List<long>> NextIdAsync(string bizType, int batchSize);
    }
}