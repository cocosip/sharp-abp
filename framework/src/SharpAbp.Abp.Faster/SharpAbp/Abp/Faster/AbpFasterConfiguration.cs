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
        /// Gets or sets the maximum number of commits that can be skipped
        /// </summary>
        public int MaxCommitSkip { get; set; } = 50;

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
