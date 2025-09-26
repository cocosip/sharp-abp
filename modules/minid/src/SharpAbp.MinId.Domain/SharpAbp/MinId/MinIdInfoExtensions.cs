﻿namespace SharpAbp.MinId
{
    /// <summary>
    /// Provides extension methods for MinIdInfo entity to support various conversion operations.
    /// These extensions facilitate data transformation between different representations
    /// of MinId configuration data, particularly for caching scenarios.
    /// </summary>
    public static class MinIdInfoExtensions
    {
        /// <summary>
        /// Converts a MinIdInfo entity to a cache item representation.
        /// This method creates a lightweight cache object that contains the essential
        /// configuration data needed for ID generation without the full entity overhead.
        /// </summary>
        /// <param name="minIdInfo">The MinIdInfo entity to convert. Cannot be null.</param>
        /// <returns>A MinIdInfoCacheItem containing the configuration data from the entity.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when minIdInfo is null.</exception>
        public static MinIdInfoCacheItem ToCacheItem(this MinIdInfo minIdInfo)
        {
            if (minIdInfo == null)
            {
                throw new System.ArgumentNullException(nameof(minIdInfo), "MinIdInfo cannot be null when converting to cache item.");
            }

            return new MinIdInfoCacheItem()
            {
                BizType = minIdInfo.BizType,
                MaxId = minIdInfo.MaxId,
                Step = minIdInfo.Step,
                Delta = minIdInfo.Delta,
                Remainder = minIdInfo.Remainder
            };
        }
    }
}
