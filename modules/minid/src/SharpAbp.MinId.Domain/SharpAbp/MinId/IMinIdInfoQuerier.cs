﻿using JetBrains.Annotations;
using System.Threading.Tasks;

namespace SharpAbp.MinId
{
    /// <summary>
    /// Defines the contract for querying MinId information with caching capabilities.
    /// This interface provides methods to check the existence and retrieve MinId configurations
    /// for different business types with optimized performance through caching mechanisms.
    /// </summary>
    public interface IMinIdInfoQuerier
    {
        /// <summary>
        /// Checks whether a MinId configuration exists for the specified business type.
        /// This method utilizes distributed caching to improve performance and reduce database queries.
        /// The result is cached for subsequent calls to enhance system responsiveness.
        /// </summary>
        /// <param name="bizType">The business type identifier to check for existence. Cannot be null or whitespace.</param>
        /// <returns>A task containing true if a MinId configuration exists for the business type; otherwise, false.</returns>
        /// <exception cref="System.ArgumentException">Thrown when bizType is null or whitespace.</exception>
        Task<bool> ExistAsync([NotNull] string bizType);
    }
}
