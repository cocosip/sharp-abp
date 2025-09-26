namespace SharpAbp.MinId
{
    /// <summary>
    /// Data transfer object representing a segment of ID values.
    /// Used to manage and track ID segments for distributed ID generation.
    /// </summary>
    public class SegmentIdDto
    {
        /// <summary>
        /// Gets or sets the current ID in the segment.
        /// Represents the last issued ID from this segment.
        /// </summary>
        public long CurrentId { get; set; }

        /// <summary>
        /// Gets or sets the maximum ID in the segment.
        /// Represents the upper bound of this segment (exclusive).
        /// </summary>
        public long MaxId { get; set; }

        /// <summary>
        /// Gets or sets the delta value for ID increment.
        /// Defines the step size for generating subsequent IDs.
        /// </summary>
        public int Delta { get; set; }

        /// <summary>
        /// Gets or sets the remainder value for ID distribution.
        /// Used to ensure IDs are distributed across segments with different remainders.
        /// </summary>
        public int Remainder { get; set; }

        /// <summary>
        /// Gets or sets the loading ID threshold.
        /// Represents the ID value at which a new segment should be pre-loaded.
        /// </summary>
        public long LoadingId { get; set; }
    }
}