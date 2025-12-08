namespace SharpAbp.Abp.Faster
{
    /// <summary>
    /// Represents a log entry with data and address range information.
    /// </summary>
    /// <typeparam name="T">The type of data stored in the log entry.</typeparam>
    public class LogEntry<T> : IAddressRange where T : class
    {
        /// <summary>
        /// Gets or sets the data stored in this log entry.
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Gets or sets the starting address of this log entry.
        /// </summary>
        public long CurrentAddress { get; set; }

        /// <summary>
        /// Gets or sets the next address after this log entry (end of range).
        /// </summary>
        public long NextAddress { get; set; }

        /// <summary>
        /// Gets the length of this log entry in bytes (computed from address range).
        /// </summary>
        public long EntryLength => NextAddress - CurrentAddress;

        /// <summary>
        /// Gets the start address (same as CurrentAddress).
        /// </summary>
        long IAddressRange.Start => CurrentAddress;

        /// <summary>
        /// Gets the end address (same as NextAddress).
        /// </summary>
        long IAddressRange.End => NextAddress;
    }

    public class BufferedLogEntry : LogEntry<byte[]>
    {

    }
}
