using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.Faster
{
    /// <summary>
    /// Represents a base interface for FASTER loggers.
    /// </summary>
    public interface IFasterLogger : IDisposable
    {

    }

    /// <summary>
    /// Represents a generic interface for FASTER loggers.
    /// </summary>
    /// <typeparam name="T">The type of objects to be logged.</typeparam>
    public interface IFasterLogger<T> : IFasterLogger where T : class
    {
        /// <summary>
        /// Gets whether the logger has been initialized.
        /// </summary>
        bool Initialized { get; }

        /// <summary>
        /// Gets the begin address of the log (earliest valid data address).
        /// </summary>
        long BeginAddress { get; }

        /// <summary>
        /// Gets the committed-until address of the log (latest durably written address).
        /// Use this as the toAddress upper bound for exports.
        /// </summary>
        long CommittedUntilAddress { get; }

        /// <summary>
        /// Gets the total number of committed ranges since initialization.
        /// </summary>
        long TotalCommittedRanges { get; }

        /// <summary>
        /// Gets the total number of writes since initialization.
        /// </summary>
        long TotalWriteCount { get; }

        /// <summary>
        /// Gets the total number of reads since initialization.
        /// </summary>
        long TotalReadCount { get; }

        /// <summary>
        /// Gets the current number of gaps in the completed ranges.
        /// </summary>
        long CurrentGapCount { get; }

        /// <summary>
        /// Gets the largest gap size in bytes detected.
        /// </summary>
        long LargestGapSize { get; }

        /// <summary>
        /// Gets the number of completed ranges currently tracked.
        /// </summary>
        int CompletedRangeCount { get; }

        /// <summary>
        /// Gets the current truncate address.
        /// </summary>
        long TruncateBeforeAddress { get; }

        /// <summary>
        /// Initializes the FASTER logger with the specified configuration.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Writes a single entity to the log.
        /// </summary>
        /// <param name="entity">The entity to write.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The position of the written entry.</returns>
        Task<long> WriteAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Writes a batch of entities to the log.
        /// </summary>
        /// <param name="values">The entities to write.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The positions of the written entries.</returns>
        Task<List<long>> BatchWriteAsync(List<T> values, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads log entries from the pending channel.
        /// </summary>
        /// <param name="count">The number of entries to read.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of log entries.</returns>
        Task<LogEntryList<T>> ReadAsync(int count = 1, CancellationToken cancellationToken = default);

        /// <summary>
        /// Commits the specified log entry positions.
        /// </summary>
        /// <param name="positions">The positions to commit.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task CommitAsync(IEnumerable<Position> positions, CancellationToken cancellationToken = default);

        /// <summary>
        /// Manually forces completion past a persistent gap by marking the gap range as processed.
        /// ⚠️ WARNING: This will cause data loss for entries in the specified gap range!
        /// Only use this method when you're certain the gap data is permanently lost or unrecoverable.
        /// </summary>
        /// <param name="gapStart">The start address of the gap to skip.</param>
        /// <param name="gapEnd">The end address of the gap to skip.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task ForceCommitGap(long gapStart, long gapEnd, CancellationToken cancellationToken = default);

        /// <summary>
        /// Exports log entries from the specified address range to files in the target directory.
        /// Uses a temporary scan iterator that does not affect the main consumer position.
        /// Large exports are automatically split into multiple files based on entriesPerFile.
        /// </summary>
        /// <param name="targetDirectory">The directory to write export files into.</param>
        /// <param name="fromAddress">The start address to export from (use BeginAddress for full export, or ToAddress from a previous result for incremental export).</param>
        /// <param name="toAddress">The end address to export to. Defaults to CommittedUntilAddress if not specified.</param>
        /// <param name="entriesPerFile">Maximum number of entries per file before splitting. Default is 10000.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Export result containing file paths and address range for next incremental export.</returns>
        Task<FasterExportResult> ExportAsync(
            string targetDirectory,
            long fromAddress,
            long? toAddress = null,
            int entriesPerFile = 10000,
            CancellationToken cancellationToken = default);
    }
}