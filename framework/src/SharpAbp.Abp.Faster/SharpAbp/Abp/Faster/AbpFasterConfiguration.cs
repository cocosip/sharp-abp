using FASTER.core;
using System;

namespace SharpAbp.Abp.Faster
{
    /// <summary>
    /// Configuration options for ABP Faster logging functionality
    /// </summary>
    public class AbpFasterConfiguration
    {
        /// <summary>
        /// Gets or sets the file name for storing log data
        /// </summary>
        public string? FileName { get; set; }

        /// <summary>
        /// Gets or sets whether to preallocate the file on creation
        /// </summary>
        public bool PreallocateFile { get; set; }

        /// <summary>
        /// Gets or sets the storage capacity in bytes (default: 4GB)
        /// </summary>
        public long Capacity { get; set; } = 1L << 32;

        /// <summary>
        /// Gets or sets whether to recover device metadata from existing files
        /// </summary>
        public bool RecoverDevice { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to use IO completion port with polling
        /// </summary>
        public bool UseIoCompletionPort { get; set; } = false;

        /// <summary>
        /// Gets or sets whether file buffering during write is disabled (default of true requires aligned writes)
        /// </summary>
        public bool DisableFileBuffering { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to scan uncommitted entries that haven't been fully written to pages
        /// </summary>
        public bool ScanUncommitted { get; set; } = false;

        /// <summary>
        /// Gets or sets whether to automatically refresh the safe tail address
        /// </summary>
        public bool AutoRefreshSafeTailAddress { get; set; } = false;

        /// <summary>
        /// Gets or sets the page size in bits. 20(1MB), native default is 22(4MB)
        /// </summary>
        public int PageSizeBits { get; set; } = 22;

        /// <summary>
        /// Gets or sets the memory size in bits. 21(2MB), native default is 23(8MB)
        /// </summary>
        public int MemorySizeBits { get; set; } = 23;

        /// <summary>
        /// Gets or sets the segment size in bits. 28(256MB), native default is 30(1GB)
        /// </summary>
        public int SegmentSizeBits { get; set; } = 30;

        /// <summary>
        /// Gets or sets the commit interval in milliseconds for data persistence
        /// </summary>
        public int CommitIntervalMillis { get; set; } = 2000;

        /// <summary>
        /// Gets or sets the completion interval in milliseconds for data processing
        /// </summary>
        public int CompleteIntervalMillis { get; set; } = 3000;

        /// <summary>
        /// Gets or sets the log truncation interval in milliseconds
        /// </summary>
        public int TruncateIntervalMillis { get; set; } = 60 * 5 * 1000;

        /// <summary>
        /// Gets or sets the pre-read capacity for data retrieved from Faster in advance
        /// </summary>
        public int PreReadCapacity { get; set; } = 5000;

        /// <summary>
        /// Gets or sets the address tolerance for gap merging in completed ranges.
        /// When merging completed ranges, gaps smaller than this value are considered continuous.
        /// This allows for small address variations in log entries to be tolerated.
        /// Default is 10 bytes.
        /// </summary>
        public long AddressMatchTolerance { get; set; } = 10;

        /// <summary>
        /// Gets or sets the timeout in milliseconds for detecting stale gaps.
        /// If a gap persists for longer than this duration, a warning is logged.
        /// Set to 0 to disable gap timeout detection.
        /// Default is 10 minutes (600000 ms).
        /// </summary>
        public int GapTimeoutMillis { get; set; } = 10 * 60 * 1000;

        /// <summary>
        /// Gets or sets the maximum number of ranges to keep in the completed ranges set.
        /// If exceeded, the oldest ranges beyond the gap will be removed to prevent memory growth.
        /// Set to 0 for unlimited (not recommended).
        /// Default is 10000.
        /// </summary>
        public int MaxCompletedRanges { get; set; } = 10000;

        /// <summary>
        /// Gets or sets the timeout in milliseconds for forcing completion past a persistent gap.
        /// If a gap persists longer than this duration, the system will automatically skip the gap
        /// and continue processing subsequent ranges. This prevents indefinite blocking but may
        /// result in data loss for the skipped range.
        /// Set to 0 to disable automatic gap skipping (manual intervention required).
        /// Default is 120000 ms (2 minutes).
        /// WARNING: Skipped gaps may cause data loss if the consumer is truly stuck!
        /// Recommended: Implement idempotent processing to handle potential reprocessing.
        /// </summary>
        public int ForceCompleteGapTimeoutMillis { get; set; } = 120000;

        /// <summary>
        /// Gets or sets the name of the iterator
        /// </summary>
        public string IteratorName { get; set; } = "default";

        /// <summary>
        /// Gets or sets the configuration action for FasterLogSettings
        /// </summary>
        public Action<FasterLogSettings>? Configure { get; set; }
    }
}
