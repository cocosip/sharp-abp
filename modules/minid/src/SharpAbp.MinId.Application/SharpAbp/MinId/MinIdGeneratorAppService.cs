﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpAbp.MinId
{
    /// <summary>
    /// Application service implementation for generating unique IDs using the MinId algorithm.
    /// Provides methods to generate single or batch IDs for a specified business type.
    /// </summary>
    public class MinIdGeneratorAppService : MinIdAppService, IMinIdGeneratorAppService
    {
        /// <summary>
        /// Factory for creating and managing MinId generators for different business types.
        /// </summary>
        protected IMinIdGeneratorFactory MinIdGeneratorFactory { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MinIdGeneratorAppService"/> class.
        /// </summary>
        /// <param name="minIdGeneratorFactory">Factory for creating and managing MinId generators.</param>
        public MinIdGeneratorAppService(IMinIdGeneratorFactory minIdGeneratorFactory)
        {
            MinIdGeneratorFactory = minIdGeneratorFactory;
        }

        /// <summary>
        /// Generates the next unique ID for the specified business type.
        /// </summary>
        /// <param name="bizType">The business type identifier for which to generate an ID.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the generated unique ID.</returns>
        public virtual async Task<long> NextIdAsync(string bizType)
        {
            var minIdGenerator = await MinIdGeneratorFactory.GetAsync(bizType);
            return await minIdGenerator.NextIdAsync();
        }

        /// <summary>
        /// Generates a batch of unique IDs for the specified business type.
        /// Ensures the batch size is at least 1 before generating IDs.
        /// </summary>
        /// <param name="bizType">The business type identifier for which to generate IDs.</param>
        /// <param name="batchSize">The number of IDs to generate in the batch. Will be adjusted to a minimum of 1 if less than 1.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of generated unique IDs.</returns>
        public virtual async Task<List<long>> NextIdAsync(string bizType, int batchSize)
        {
            batchSize = batchSize < 1 ? 1 : batchSize;
            var minIdGenerator = await MinIdGeneratorFactory.GetAsync(bizType);
            return await minIdGenerator.NextIdAsync(batchSize);
        }
    }
}