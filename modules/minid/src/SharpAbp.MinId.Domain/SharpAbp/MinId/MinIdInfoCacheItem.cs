﻿namespace SharpAbp.MinId
{
    /// <summary>
    /// Represents a cached version of MinId information for improved performance.
    /// This cache item contains the essential configuration data needed for MinId generation
    /// without the overhead of full entity loading from the database.
    /// </summary>
    public class MinIdInfoCacheItem
    {
        /// <summary>
        /// Gets or sets the business type identifier for this MinId configuration.
        /// This value uniquely identifies the context for which IDs are generated.
        /// </summary>
        public string BizType { get; set; }

        /// <summary>
        /// Gets or sets the current maximum ID value that has been allocated.
        /// This represents the upper bound of the current ID allocation range.
        /// </summary>
        public long MaxId { get; set; }

        /// <summary>
        /// Gets or sets the step size for ID allocation increments.
        /// This determines how many IDs are allocated in each segment during distribution.
        /// </summary>
        public int Step { get; set; }

        /// <summary>
        /// Gets or sets the delta value used for modulo operations in distributed environments.
        /// This ensures proper ID distribution across multiple service instances.
        /// </summary>
        public int Delta { get; set; }

        /// <summary>
        /// Gets or sets the remainder value used with delta for modulo calculations.
        /// This helps maintain uniqueness and proper distribution in clustered deployments.
        /// </summary>
        public int Remainder { get; set; }
    }
}
