namespace SharpAbp.Abp.Faster
{
    public class LogEntry<T> where T : class
    {
        /// <summary>
        /// Data
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// EntryLength
        /// </summary>
        public long EntryLength { get; set; }

        /// <summary>
        /// Current addreess
        /// </summary>
        public long CurrentAddress { get; set; }
    }

    public class BufferedLogEntry : LogEntry<byte[]>
    {

    }
}
