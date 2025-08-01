namespace SharpAbp.Abp.Snowflakes
{
    /// <summary>
    /// Represents the configuration for a Snowflake ID generator instance.
    /// </summary>
    public class SnowflakeConfiguration
    {
        /// <summary>
        /// Gets or sets the custom epoch timestamp in milliseconds.
        /// This is the starting point for time calculations in the Snowflake algorithm.
        /// Default is January 1, 2015, 00:00:00 UTC (1420041600000L).
        /// </summary>
        public long Twepoch { get; set; } = 1420041600000L;

        /// <summary>
        /// Gets or sets the number of bits allocated for the worker ID.
        /// Default is 5 bits, allowing for 32 worker IDs (0-31).
        /// </summary>
        public int WorkerIdBits { get; set; } = 5;

        /// <summary>
        /// Gets or sets the number of bits allocated for the datacenter ID.
        /// Default is 5 bits, allowing for 32 datacenter IDs (0-31).
        /// </summary>
        public int DatacenterIdBits { get; set; } = 5;

        /// <summary>
        /// Gets or sets the number of bits allocated for the sequence number.
        /// This determines the number of IDs that can be generated within a single millisecond.
        /// Default is 12 bits, allowing for 4096 sequence numbers (0-4095).
        /// </summary>
        public int SequenceBits { get; set; } = 12;

        /// <summary>
        /// Gets or sets the worker ID for this Snowflake instance.
        /// This ID must be unique within its datacenter.
        /// Default is 0.
        /// </summary>
        public long WorkerId { get; set; } = 0L;

        /// <summary>
        /// Gets or sets the datacenter ID for this Snowflake instance.
        /// This ID must be unique across all datacenters.
        /// Default is 0.
        /// </summary>
        public long DatacenterId { get; set; } = 0L;
    }
}
