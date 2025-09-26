﻿using System.Threading.Tasks;

namespace SharpAbp.MinId
{
    /// <summary>
    /// Factory interface for creating and managing MinId generators.
    /// </summary>
    public interface IMinIdGeneratorFactory
    {
        /// <summary>
        /// Gets a MinId generator instance for the specified business type.
        /// </summary>
        /// <param name="bizType">The business type identifier for which to retrieve the generator.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the MinId generator instance.</returns>
        Task<IMinIdGenerator> GetAsync(string bizType);
    }
}