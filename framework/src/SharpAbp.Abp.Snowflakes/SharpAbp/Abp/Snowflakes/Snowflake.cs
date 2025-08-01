using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.Snowflakes
{
    /// <summary>
    /// Implements the Snowflake algorithm for generating unique, distributed IDs.
    /// The ID structure is as follows:
    /// 0 - 0000000000 0000000000 0000000000 0000000000 0 - 00000 - 00000 - 000000000000
    /// 1 bit: Sign bit (always 0 for positive IDs).
    /// 41 bits: Timestamp (milliseconds since a custom epoch). This allows for ~69 years of IDs.
    /// 10 bits: Machine ID (5 bits for datacenter ID, 5 bits for worker ID), supporting 1024 nodes.
    /// 12 bits: Sequence number (milliseconds within the same timestamp), supporting 4096 IDs per millisecond per node.
    /// Total: 64 bits (long type).
    /// </summary>
    public class Snowflake
    {
        // Custom epoch (January 1, 2015, 00:00:00 UTC)
        private readonly long _epoch;

        // Number of bits allocated for worker ID
        private readonly int _workerIdBits;

        // Number of bits allocated for datacenter ID
        private readonly int _datacenterIdBits;

        // Number of bits allocated for sequence number
        private readonly int _sequenceBits;

        // Shift left for worker ID
        private readonly int _workerIdShift;

        // Shift left for datacenter ID
        private readonly int _datacenterIdShift;

        // Shift left for timestamp
        private readonly int _timestampLeftShift;

        // Mask for sequence number
        private readonly long _sequenceMask;

        // Worker ID (0-31)
        private readonly long _workerId;

        // Datacenter ID (0-31)
        private readonly long _datacenterId;

        // Sequence number within the millisecond (0-4095)
        private long _sequence = 0L;

        // Last timestamp when an ID was generated
        private long _lastTimestamp = -1L;

        // Object for thread synchronization
        private readonly object _syncObject = new object();

        // Default static instance of Snowflake for convenience
        private static readonly Lazy<Snowflake> _defaultInstance = new Lazy<Snowflake>(() => new Snowflake());

        /// <summary>
        /// Gets the default static instance of the <see cref="Snowflake"/> ID generator.
        /// This instance uses default parameters (workerId = 0, datacenterId = 0).
        /// </summary>
        public static Snowflake Default => _defaultInstance.Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Snowflake"/> class with default parameters.
        /// Uses a default epoch (January 1, 2015, 00:00:00 UTC), 5 bits for worker ID, 5 bits for datacenter ID, and 12 bits for sequence.
        /// </summary>
        /// <param name="workerId">The worker ID (0-31). Defaults to 0.</param>
        /// <param name="datacenterId">The datacenter ID (0-31). Defaults to 0.</param>
        public Snowflake(long workerId = 0L, long datacenterId = 0L)
            : this(1420041600000L, 5, 5, 12, workerId, datacenterId)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Snowflake"/> class with custom parameters.
        /// </summary>
        /// <param name="epoch">The custom epoch timestamp in milliseconds.</param>
        /// <param name="workerIdBits">The number of bits to use for the worker ID (e.g., 5).</param>
        /// <param name="datacenterIdBits">The number of bits to use for the datacenter ID (e.g., 5).</param>
        /// <param name="sequenceBits">The number of bits to use for the sequence number (e.g., 12).</param>
        /// <param name="workerId">The worker ID.</param>
        /// <param name="datacenterId">The datacenter ID.</param>
        /// <exception cref="ArgumentException">Thrown if workerId or datacenterId are out of their valid ranges based on the bit allocations.</exception>
        public Snowflake(
            long epoch,
            int workerIdBits,
            int datacenterIdBits,
            int sequenceBits,
            long workerId,
            long datacenterId)
        {
            _epoch = epoch;
            _workerIdBits = workerIdBits;
            _datacenterIdBits = datacenterIdBits;
            _sequenceBits = sequenceBits;

            _workerIdShift = _sequenceBits;
            _datacenterIdShift = _sequenceBits + _workerIdBits;
            _timestampLeftShift = _sequenceBits + _workerIdBits + _datacenterIdBits;
            _sequenceMask = -1L ^ (-1L << _sequenceBits);

            long maxWorkerId = -1L ^ (-1L << _workerIdBits);
            long maxDatacenterId = -1L ^ (-1L << _datacenterIdBits);

            if (workerId > maxWorkerId || workerId < 0)
            {
                throw new ArgumentException($"Worker ID can't be greater than {maxWorkerId} or less than 0.", nameof(workerId));
            }
            if (datacenterId > maxDatacenterId || datacenterId < 0)
            {
                throw new ArgumentException($"Datacenter ID can't be greater than {maxDatacenterId} or less than 0.", nameof(datacenterId));
            }

            _workerId = workerId;
            _datacenterId = datacenterId;
        }

        /// <summary>
        /// Generates the next unique ID. This method is thread-safe.
        /// </summary>
        /// <returns>The next unique 64-bit ID.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the system clock moves backwards, indicating a potential issue with time synchronization.</exception>
        public long NextId()
        {
            lock (_syncObject)
            {
                long timestamp = GetCurrentTimestampMillis();

                // If current time is less than last generated timestamp, clock has moved backwards.
                if (timestamp < _lastTimestamp)
                {
                    throw new InvalidOperationException(
                            $"Clock moved backwards. Refusing to generate ID for {_lastTimestamp - timestamp} milliseconds.");
                }

                // If generated in the same millisecond, increment sequence.
                if (_lastTimestamp == timestamp)
                {
                    _sequence = (_sequence + 1) & _sequenceMask;
                    // If sequence overflows (reaches 0), block until next millisecond.
                    if (_sequence == 0)
                    {
                        timestamp = WaitUntilNextMillis(_lastTimestamp);
                    }
                }
                // If timestamp has changed, reset sequence.
                else
                {
                    _sequence = 0L;
                }

                _lastTimestamp = timestamp;

                // Combine parts to form the 64-bit ID.
                return ((timestamp - _epoch) << _timestampLeftShift) |
                       (_datacenterId << _datacenterIdShift) |
                       (_workerId << _workerIdShift) |
                       _sequence;
            }
        }

        /// <summary>
        /// Blocks until the next millisecond to ensure unique timestamps.
        /// </summary>
        /// <param name="lastTimestamp">The last timestamp when an ID was generated.</param>
        /// <returns>The new timestamp in milliseconds.</returns>
        private long WaitUntilNextMillis(long lastTimestamp)
        {
            long timestamp = GetCurrentTimestampMillis();
            while (timestamp <= lastTimestamp)
            {
                timestamp = GetCurrentTimestampMillis();
            }
            return timestamp;
        }

        /// <summary>
        /// Gets the current timestamp in milliseconds since the Unix epoch (January 1, 1970, 00:00:00 UTC).
        /// </summary>
        /// <returns>The current timestamp in milliseconds.</returns>
        protected virtual long GetCurrentTimestampMillis()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }
    }
}