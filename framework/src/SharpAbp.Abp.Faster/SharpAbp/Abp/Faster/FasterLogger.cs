using FASTER.core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Volo.Abp.Json;
using Volo.Abp.Reflection;
using Volo.Abp.Serialization;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.Faster
{
    /// <summary>
    /// A high-performance logger implementation using FASTER for efficient logging operations.
    /// </summary>
    /// <typeparam name="T">The type of objects to be logged.</typeparam>
    public class FasterLogger<T> : IDisposable, IFasterLogger<T> where T : class
    {
        private readonly SemaphoreSlim _commitSemaphore = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _initializeSemaphore = new SemaphoreSlim(1, 1);
        private readonly CancellationTokenSource _disposeCts = new CancellationTokenSource();
        private CancellationTokenSource? _linkedCts;
        private long _truncateBeforeAddress = -1L;
        private bool _initialized = false;
        private bool _disposed = false;

        // Use interval tracking instead of retry-based commit tracking
        private readonly SortedSet<CompletedRange> _completedRanges = [];
        private readonly object _completedRangesLock = new object();
        private DateTime _lastGapWarningTime = DateTime.MinValue;
        private long _lastGapStart = -1;

        // Background task tracking for graceful shutdown
        private readonly List<Task> _backgroundTasks = new List<Task>();

        // Persistence tracking
        private bool _rangesModified = false;
        private DateTime _lastPersistTime = DateTime.MinValue;

        // Monitoring metrics for observability
        private long _totalCommittedRanges = 0;
        private long _totalWriteCount = 0;
        private long _totalReadCount = 0;
        private long _currentGapCount = 0;
        private long _largestGapSize = 0;

        protected readonly Channel<BufferedLogEntry> _pendingChannel;

        protected ILogger Logger { get; }
        protected AbpFasterOptions Options { get; }
        protected AbpFasterConfiguration Configuration { get; }
        protected IObjectSerializer Serializer { get; }
        protected ICancellationTokenProvider CancellationTokenProvider { get; }
        protected FasterLog? Log { get; set; }
        protected FasterLogScanIterator? Iter { get; set; }

        /// <summary>
        /// Gets the combined cancellation token from both the dispose CTS and the application CTS.
        /// </summary>
        protected CancellationToken CombinedCancellationToken => _linkedCts?.Token ?? CancellationToken.None;

        /// <summary>
        /// Gets whether the logger has been initialized.
        /// </summary>
        public bool Initialized => _initialized;

        /// <summary>
        /// Gets the name of the logger.
        /// </summary>
        public string? Name { get; }

        /// <summary>
        /// Gets the total number of committed ranges since initialization.
        /// </summary>
        public long TotalCommittedRanges => Interlocked.Read(ref _totalCommittedRanges);

        /// <summary>
        /// Gets the total number of writes since initialization.
        /// </summary>
        public long TotalWriteCount => Interlocked.Read(ref _totalWriteCount);

        /// <summary>
        /// Gets the total number of reads since initialization.
        /// </summary>
        public long TotalReadCount => Interlocked.Read(ref _totalReadCount);

        /// <summary>
        /// Gets the current number of gaps in the completed ranges.
        /// </summary>
        public long CurrentGapCount => Interlocked.Read(ref _currentGapCount);

        /// <summary>
        /// Gets the largest gap size in bytes detected.
        /// </summary>
        public long LargestGapSize => Interlocked.Read(ref _largestGapSize);

        /// <summary>
        /// Gets the number of completed ranges currently tracked.
        /// </summary>
        public int CompletedRangeCount
        {
            get
            {
                lock (_completedRangesLock)
                {
                    return _completedRanges.Count;
                }
            }
        }

        /// <summary>
        /// Gets the current truncate address.
        /// </summary>
        public long TruncateBeforeAddress => Interlocked.Read(ref _truncateBeforeAddress);

        /// <summary>
        /// Initializes a new instance of the <see cref="FasterLogger{T}"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="options">The FASTER options.</param>
        /// <param name="name">The logger name.</param>
        /// <param name="configuration">The FASTER configuration.</param>
        /// <param name="serializer">The object serializer.</param>
        /// <param name="cancellationTokenProvider">The cancellation token provider.</param>
        public FasterLogger(
            ILogger<FasterLogger<T>> logger,
            IOptions<AbpFasterOptions> options,
            string name,
            AbpFasterConfiguration configuration,
            IObjectSerializer serializer,
            ICancellationTokenProvider cancellationTokenProvider)
        {
            Logger = logger;
            Options = options.Value;
            Name = name;
            Configuration = configuration;
            Serializer = serializer;
            CancellationTokenProvider = cancellationTokenProvider;

            _linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                _disposeCts.Token,
                cancellationTokenProvider.Token);

            // Create bounded channel with explicit full mode configuration
            // Use Wait mode to ensure no data loss when channel is full
            var channelOptions = new BoundedChannelOptions(Configuration.PreReadCapacity)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,  // Only one ReadAsync consumer
                SingleWriter = true   // Only one scan task writing
            };
            _pendingChannel = Channel.CreateBounded<BufferedLogEntry>(channelOptions);
        }

        /// <summary>
        /// Initializes the FASTER logger with the specified configuration.
        /// </summary>
        public virtual void Initialize()
        {
            // Use double-checked locking pattern for thread-safe initialization
            if (_initialized)
            {
                return;
            }

            _initializeSemaphore.Wait();
            try
            {
                // Double-check after acquiring the lock
                if (_initialized)
                {
                    Logger.LogWarning(
                        "FASTER logger '{LoggerName}' is already initialized.",
                        TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)));
                    return;
                }

                // Validate configuration
                if (string.IsNullOrWhiteSpace(Options.RootPath))
                {
                    throw new InvalidOperationException(
                        $"RootPath must be configured in AbpFasterOptions for logger type '{TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T))}'.");
                }

                if (string.IsNullOrWhiteSpace(Configuration.FileName))
                {
                    throw new InvalidOperationException(
                        $"FileName must be configured for logger type '{TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T))}'.");
                }

                var fileName = Path.Combine(
                    Options.RootPath,
                    TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)),
                    Configuration.FileName);

                var device = Devices.CreateLogDevice(
                    fileName,
                    Configuration.PreallocateFile,
                    false,
                    Configuration.Capacity,
                    Configuration.RecoverDevice,
                    Configuration.UseIoCompletionPort,
                    Configuration.DisableFileBuffering);

                var settings = new FasterLogSettings()
                {
                    LogDevice = device,
                    PageSizeBits = Configuration.PageSizeBits,
                    MemorySizeBits = Configuration.MemorySizeBits,
                    SegmentSizeBits = Configuration.SegmentSizeBits,
                    AutoRefreshSafeTailAddress = Configuration.AutoRefreshSafeTailAddress,
                    AutoCommit = false
                };

                Configuration.Configure?.Invoke(settings);
                Log = new FasterLog(settings);

                Iter = Log.Scan(
                    Log.BeginAddress,
                    long.MaxValue,
                    Configuration.IteratorName,
                    true,
                    ScanBufferingMode.DoublePageBuffering,
                    Configuration.ScanUncommitted,
                    Logger);

                Interlocked.Exchange(ref _truncateBeforeAddress, Math.Max(Iter.CompletedUntilAddress, Iter.BeginAddress));

                // Load persisted completed ranges if enabled
                if (Configuration.EnableRangePersistence)
                {
                    LoadCompletedRanges();
                }

                // Start background tasks
                StartScheduleCommit();
                StartScheduleScan();
                StartScheduleComplete();
                StartScheduleTruncate();

                // Start persistence task if enabled
                if (Configuration.EnableRangePersistence)
                {
                    StartSchedulePersist();
                }

                _initialized = true;

                Logger.LogInformation(
                    "FASTER logger '{LoggerName}' initialized successfully. Begin address: {BeginAddress}, Truncate before address: {TruncateAddress}, File: {FileName}.",
                    TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)),
                    Iter.BeginAddress,
                    _truncateBeforeAddress,
                    fileName);
            }
            finally
            {
                _initializeSemaphore.Release();
            }
        }

        /// <summary>
        /// Starts the scheduled commit task to periodically commit written data.
        /// This prevents data loss due to unexpected shutdowns.
        /// </summary>
        protected virtual void StartScheduleCommit()
        {
            var task = Task.Factory.StartNew(async () =>
            {
                while (!CombinedCancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        await _commitSemaphore.WaitAsync(CombinedCancellationToken);
                        if (Log != null)
                        {
                            await Log.CommitAsync(CombinedCancellationToken);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // Expected during shutdown, no logging needed
                        break;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Error committing FASTER log data for type '{TypeName}': {Message}",
                            TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)), ex.Message);
                    }
                    finally
                    {
                        _commitSemaphore.Release();
                    }

                    // Delay after the commit attempt to avoid initial delay
                    await Task.Delay(Configuration.CommitIntervalMillis, CombinedCancellationToken);
                }
            }, TaskCreationOptions.LongRunning).Unwrap();

            _backgroundTasks.Add(task);
        }

        /// <summary>
        /// Starts the scheduled scan task to read data from the log iterator.
        /// </summary>
        protected virtual void StartScheduleScan()
        {
            var task = Task.Factory.StartNew(async () =>
            {
                while (!CombinedCancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        byte[]? buffer = null;
                        int entryLength = 0;
                        long currentAddress = 0;
                        long nextAddress = 0;

                        if (Iter != null)
                        {
                            while (!Iter.GetNext(out buffer, out entryLength, out currentAddress, out nextAddress))
                            {
                                await Iter.WaitAsync(CombinedCancellationToken);
                            }
                        }

                        if (buffer != null && entryLength > 0)
                        {
                            var entry = new BufferedLogEntry
                            {
                                Data = buffer,
                                CurrentAddress = currentAddress,
                                NextAddress = nextAddress
                            };

                            await _pendingChannel.Writer.WriteAsync(entry, CombinedCancellationToken);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // Expected during shutdown, no logging needed
                        break;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Error during scheduled log scan for type '{TypeName}': {Message}",
                            TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)), ex.Message);

                        // Add delay to prevent rapid failure loop
                        await Task.Delay(1000, CombinedCancellationToken);
                    }
                }
            }, TaskCreationOptions.LongRunning).Unwrap();

            _backgroundTasks.Add(task);
        }

        /// <summary>
        /// Starts the scheduled completion task to process completed log entries.
        /// Uses interval merging to find the largest continuous range from the beginning.
        /// </summary>
        protected virtual void StartScheduleComplete()
        {
            var task = Task.Factory.StartNew(async () =>
            {
                while (!CombinedCancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        long currentTruncateAddress = Interlocked.Read(ref _truncateBeforeAddress);
                        long newTruncateAddress = currentTruncateAddress;
                        var rangesToRemove = new List<CompletedRange>();

                        lock (_completedRangesLock)
                        {
                            if (_completedRanges.Count > 0)
                            {
                                // Find the largest continuous range from the beginning
                                // Tolerance allows small gaps to be considered continuous
                                long currentEnd = currentTruncateAddress;

                                foreach (var range in _completedRanges)
                                {
                                    // Check if this range connects to or overlaps with the current continuous end
                                    // Allow tolerance for small gaps
                                    if (range.Start <= currentEnd + Configuration.AddressMatchTolerance)
                                    {
                                        // Extend the continuous range
                                        currentEnd = Math.Max(currentEnd, range.End);
                                        rangesToRemove.Add(range);
                                    }
                                    else
                                    {
                                        // Gap detected
                                        long gapSize = range.Start - currentEnd;

                                        // Update gap metrics
                                        Interlocked.Exchange(ref _largestGapSize, Math.Max(Interlocked.Read(ref _largestGapSize), gapSize));

                                        // Check for stale gap timeout
                                        bool shouldForceComplete = false;
                                        if (Configuration.GapTimeoutMillis > 0)
                                        {
                                            bool isNewGap = _lastGapStart != range.Start;

                                            if (isNewGap)
                                            {
                                                _lastGapStart = range.Start;
                                                _lastGapWarningTime = DateTime.UtcNow;
                                            }
                                            else
                                            {
                                                var gapDuration = DateTime.UtcNow - _lastGapWarningTime;
                                                if (gapDuration.TotalMilliseconds > Configuration.GapTimeoutMillis)
                                                {
                                                    Logger.LogWarning(
                                                        "Gap at address {GapStart} has persisted for {Duration:F1} seconds (size: {GapSize} bytes) for type '{TypeName}'. " +
                                                        "This may indicate missing data or out-of-order processing issues.",
                                                        range.Start,
                                                        gapDuration.TotalSeconds,
                                                        gapSize,
                                                        TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)));

                                                    // Check if we should force complete past this gap
                                                    if (Configuration.ForceCompleteGapTimeoutMillis > 0 &&
                                                        gapDuration.TotalMilliseconds > Configuration.ForceCompleteGapTimeoutMillis)
                                                    {
                                                        shouldForceComplete = true;
                                                    }
                                                    else
                                                    {
                                                        // Reset timer to avoid repeated warnings
                                                        _lastGapWarningTime = DateTime.UtcNow;
                                                    }
                                                }
                                            }
                                        }

                                        // Force complete past the gap if timeout exceeded
                                        if (shouldForceComplete)
                                        {
                                            Logger.LogError(
                                                "FORCING COMPLETION past gap at address {GapStart} (size: {GapSize} bytes) for type '{TypeName}' " +
                                                "due to timeout of {Duration:F1} seconds. " +
                                                "⚠️ DATA IN RANGE [{GapStart}, {GapEnd}) WILL BE SKIPPED AND MAY BE LOST! " +
                                                "Current truncate: {CurrentEnd}, Next range starts at: {RangeStart}",
                                                currentEnd,
                                                gapSize,
                                                TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)),
                                                (DateTime.UtcNow - _lastGapWarningTime).TotalSeconds,
                                                currentEnd,
                                                range.Start,
                                                currentEnd,
                                                range.Start);

                                            // Skip the gap and continue merging from this range
                                            currentEnd = Math.Max(currentEnd, range.End);
                                            rangesToRemove.Add(range);

                                            // Reset gap tracking
                                            _lastGapStart = -1;
                                            _lastGapWarningTime = DateTime.MinValue;

                                            // Continue to merge subsequent ranges
                                            continue;
                                        }

                                        // Normal gap handling - stop merging
                                        break;
                                    }
                                }

                                newTruncateAddress = currentEnd;

                                // Don't remove ranges yet - wait until CompleteUntilRecordAtAsync succeeds
                            }
                        }

                        // Commit and complete if we have progress
                        long currentTruncate = Interlocked.Read(ref _truncateBeforeAddress);
                        if (newTruncateAddress > currentTruncate && Iter != null)
                        {
                            // Try to complete the records first
                            await CompleteUntilRecordAtAsync(newTruncateAddress);

                            // Only if successful, update the address and remove ranges
                            Interlocked.Exchange(ref _truncateBeforeAddress, newTruncateAddress);

                            lock (_completedRangesLock)
                            {
                                // Now safe to remove the merged ranges
                                foreach (var range in rangesToRemove)
                                {
                                    _completedRanges.Remove(range);
                                }

                                // Check if we have too many ranges (memory protection)
                                if (Configuration.MaxCompletedRanges > 0 &&
                                    _completedRanges.Count > Configuration.MaxCompletedRanges)
                                {
                                    int excessCount = _completedRanges.Count - Configuration.MaxCompletedRanges;
                                    var rangesArray = _completedRanges.ToArray();

                                    // Remove the oldest ranges from the beginning (they have gaps and can't be merged)
                                    // Keep ranges from the end (most recent, more likely to merge in the future)
                                    for (int i = 0; i < excessCount; i++)
                                    {
                                        _completedRanges.Remove(rangesArray[i]);
                                    }

                                    Logger.LogWarning(
                                        "Removed {Count} excess ranges from completed set for type '{TypeName}' to prevent memory growth. " +
                                        "Removed ranges: [{FirstStart}-{LastEnd}), Kept ranges start from: {KeptStart}. " +
                                        "Total ranges: {Total}, Max allowed: {Max}",
                                        excessCount,
                                        TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)),
                                        rangesArray[0].Start,
                                        rangesArray[excessCount - 1].End,
                                        excessCount < rangesArray.Length ? rangesArray[excessCount].Start : -1,
                                        _completedRanges.Count,
                                        Configuration.MaxCompletedRanges);
                                }

                                // Update gap count metric
                                Interlocked.Exchange(ref _currentGapCount, _completedRanges.Count > 0 ? _completedRanges.Count : 0);
                            }

                            Logger.LogInformation(
                                "Advanced truncate address to {TruncateAddress} for type '{TypeName}'. Removed {Count} ranges.",
                                newTruncateAddress,
                                TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)),
                                rangesToRemove.Count);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // Expected during shutdown, no logging needed
                        break;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Error during commit completion for type '{TypeName}': {Message}",
                            TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)), ex.Message);

                        // Add delay to prevent rapid failure loop (delay happens below)
                    }

                    await Task.Delay(Configuration.CompleteIntervalMillis, CombinedCancellationToken);
                }
            }, TaskCreationOptions.LongRunning).Unwrap();

            _backgroundTasks.Add(task);
        }

        /// <summary>
        /// Completes records up to the specified address.
        /// </summary>
        /// <param name="commitAddress">The address up to which records should be completed.</param>
        private async Task CompleteUntilRecordAtAsync(long commitAddress)
        {
            await _commitSemaphore.WaitAsync(CombinedCancellationToken);
            try
            {
                if (Log != null)
                {
                    await Log.CommitAsync(CombinedCancellationToken);
                }

                if (Iter != null)
                {
                    await Iter.CompleteUntilRecordAtAsync(commitAddress, CombinedCancellationToken);
                }
            }
            finally
            {
                _commitSemaphore.Release();
            }
        }

        /// <summary>
        /// Starts the scheduled truncation task to remove completed log segments.
        /// </summary>
        protected virtual void StartScheduleTruncate()
        {
            var task = Task.Factory.StartNew(async () =>
            {
                long lastTruncateAddress = -1L;

                while (!CombinedCancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        long currentTruncateAddress = Interlocked.Read(ref _truncateBeforeAddress);

                        if (currentTruncateAddress > lastTruncateAddress && Log != null)
                        {
                            Log.TruncateUntilPageStart(currentTruncateAddress);
                            Logger.LogInformation(
                                "Truncated log until page start. Address: {TruncateAddress} for type '{TypeName}'.",
                                currentTruncateAddress,
                                TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)));
                            lastTruncateAddress = currentTruncateAddress;
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // Expected during shutdown, no logging needed
                        break;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Error during log truncation for type '{TypeName}': {Message}",
                            TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)), ex.Message);

                        // Add delay to prevent rapid failure loop (delay happens below)
                    }

                    await Task.Delay(Configuration.TruncateIntervalMillis, CombinedCancellationToken);
                }
            }, TaskCreationOptions.LongRunning).Unwrap();

            _backgroundTasks.Add(task);
        }

        /// <summary>
        /// Writes a single entity to the log.
        /// </summary>
        /// <param name="entity">The entity to write.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The position of the written entry.</returns>
        public virtual async Task<long> WriteAsync(T entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var buffer = Serializer.Serialize(entity);
            if (Log == null)
            {
                throw new InvalidOperationException("Logger has not been initialized.");
            }

            var address = await Log.EnqueueAsync(buffer, cancellationToken);

            // Update metrics
            Interlocked.Increment(ref _totalWriteCount);

            return address;
        }

        /// <summary>
        /// Writes a batch of entities to the log.
        /// </summary>
        /// <param name="values">The entities to write.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The positions of the written entries.</returns>
        /// <remarks>
        /// Note: This method currently serializes and enqueues entries sequentially.
        /// FASTER's FasterLog.EnqueueAsync is already optimized for high throughput,
        /// and batching is handled internally by the commit mechanism.
        /// </remarks>
        public virtual async Task<List<long>> BatchWriteAsync(List<T> values, CancellationToken cancellationToken = default)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (Log == null)
            {
                throw new InvalidOperationException("Logger has not been initialized.");
            }

            // Pre-allocate the list with known capacity to avoid resizing
            var positions = new List<long>(values.Count);

            // Serialize and enqueue all entries
            // Note: EnqueueAsync is already optimized for sequential writes
            foreach (var entity in values)
            {
                var buffer = Serializer.Serialize(entity);
                var position = await Log.EnqueueAsync(buffer, cancellationToken);
                positions.Add(position);
            }

            // Update metrics
            Interlocked.Add(ref _totalWriteCount, values.Count);

            return positions;
        }

        /// <summary>
        /// Reads log entries from the pending channel.
        /// </summary>
        /// <param name="count">The number of entries to read.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of log entries.</returns>
        public virtual async Task<LogEntryList<T>> ReadAsync(int count = 1, CancellationToken cancellationToken = default)
        {
            var entries = new LogEntryList<T>();

            for (var i = 0; i < count; i++)
            {
                BufferedLogEntry entry;
                if (i == 0)
                {
                    entry = await _pendingChannel.Reader.ReadAsync(cancellationToken);
                }
                else
                {
                    if (!_pendingChannel.Reader.TryRead(out entry!))
                    {
                        break;
                    }
                }

                if (entry.Data != null)
                {
                    entries.Add(new LogEntry<T>
                    {
                        Data = Serializer.Deserialize<T>(entry.Data),
                        CurrentAddress = entry.CurrentAddress,
                        NextAddress = entry.NextAddress,
                    });
                }
            }

            // Update metrics
            Interlocked.Add(ref _totalReadCount, entries.Count);

            return entries;
        }

        /// <summary>
        /// Commits the specified log entry positions by adding them to the completed ranges.
        /// The background task will merge these ranges and advance the truncation point.
        /// </summary>
        /// <param name="positions">The positions to commit.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public virtual Task CommitAsync(IEnumerable<Position> positions, CancellationToken cancellationToken = default)
        {
            if (positions == null)
            {
                throw new ArgumentNullException(nameof(positions));
            }

            var positionList = positions as ICollection<Position> ?? positions.ToList();

            int invalidCount = 0;
            int duplicateCount = 0;
            int addedCount = 0;

            lock (_completedRangesLock)
            {
                foreach (var position in positionList)
                {
                    // Validate position before adding
                    if (!position.IsValid())
                    {
                        invalidCount++;
                        continue;
                    }

                    var range = new CompletedRange(position);

                    // Check for duplicate (though SortedSet handles this)
                    if (!_completedRanges.Add(range))
                    {
                        duplicateCount++;
                        continue;
                    }

                    // Update metrics
                    Interlocked.Increment(ref _totalCommittedRanges);
                    addedCount++;

                    // Mark ranges as modified for persistence
                    _rangesModified = true;
                }

                // Only log if there were issues or at debug level
                if (invalidCount > 0)
                {
                    Logger.LogWarning(
                        "Skipped {InvalidCount} invalid positions during commit for type '{TypeName}'.",
                        invalidCount,
                        TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)));
                }

                if (Logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug))
                {
                    Logger.LogDebug(
                        "Committed {AddedCount} positions ({DuplicateCount} duplicates, {InvalidCount} invalid) for type '{TypeName}'. Total ranges: {TotalRanges}",
                        addedCount,
                        duplicateCount,
                        invalidCount,
                        TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)),
                        _completedRanges.Count);
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Manually forces completion past a persistent gap by marking the gap range as processed.
        /// ⚠️ WARNING: This will cause data loss for entries in the specified gap range!
        /// Only use this method when you're certain the gap data is permanently lost or unrecoverable.
        /// </summary>
        /// <param name="gapStart">The start address of the gap to skip.</param>
        /// <param name="gapEnd">The end address of the gap to skip.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="ArgumentException">Thrown when gapStart or gapEnd are invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown when logger is not initialized.</exception>
        public virtual Task ForceCommitGap(long gapStart, long gapEnd, CancellationToken cancellationToken = default)
        {
            if (!_initialized)
            {
                throw new InvalidOperationException("Logger must be initialized before calling ForceCommitGap.");
            }

            if (gapStart < 0)
            {
                throw new ArgumentException($"Gap start address must be non-negative. Got: {gapStart}", nameof(gapStart));
            }

            if (gapEnd <= gapStart)
            {
                throw new ArgumentException(
                    $"Gap end address must be greater than gap start. Start: {gapStart}, End: {gapEnd}",
                    nameof(gapEnd));
            }

            lock (_completedRangesLock)
            {
                // Create a completed range for the gap
                var gapRange = new CompletedRange(gapStart, gapEnd);

                // Add it to completed ranges
                if (_completedRanges.Add(gapRange))
                {
                    _rangesModified = true;

                    Logger.LogError(
                        "⚠️ MANUAL GAP FILL: Forcibly marked gap [{GapStart}, {GapEnd}) as completed. " +
                        "Data in this range will be SKIPPED and MAY BE LOST! " +
                        "Gap size: {GapSize} bytes. Type: '{TypeName}'",
                        gapStart,
                        gapEnd,
                        gapEnd - gapStart,
                        TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)));

                    // Reset gap tracking since we just filled it
                    if (_lastGapStart == gapStart)
                    {
                        _lastGapStart = -1;
                        _lastGapWarningTime = DateTime.MinValue;
                    }
                }
                else
                {
                    Logger.LogWarning(
                        "Gap range [{GapStart}, {GapEnd}) was already in completed ranges. Type: '{TypeName}'",
                        gapStart,
                        gapEnd,
                        TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)));
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Starts the scheduled persistence task to save completed ranges periodically.
        /// </summary>
        protected virtual void StartSchedulePersist()
        {
            var task = Task.Factory.StartNew(async () =>
            {
                while (!CombinedCancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(Configuration.PersistIntervalMillis, CombinedCancellationToken);

                        // Only persist if data has changed
                        if (_rangesModified)
                        {
                            await PersistCompletedRangesAsync();
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // Final save before exit
                        if (_rangesModified)
                        {
                            await PersistCompletedRangesAsync();
                        }
                        break;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Error persisting completed ranges for type '{TypeName}': {Message}",
                            TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)), ex.Message);

                        // Add delay to prevent rapid failure loop
                        await Task.Delay(1000, CombinedCancellationToken);
                    }
                }
            }, TaskCreationOptions.LongRunning).Unwrap();

            _backgroundTasks.Add(task);
        }

        /// <summary>
        /// Persists the completed ranges to disk.
        /// Only persists ranges that are beyond the current truncate address to reduce file size.
        /// </summary>
        private async Task PersistCompletedRangesAsync()
        {
            List<CompletedRange> snapshot;
            long currentTruncate;

            lock (_completedRangesLock)
            {
                if (_completedRanges.Count == 0)
                {
                    // No data to persist
                    _rangesModified = false;
                    return;
                }

                currentTruncate = Interlocked.Read(ref _truncateBeforeAddress);

                // Only persist ranges beyond the current truncate address
                // Ranges before truncate address are already committed and don't need recovery
                snapshot = [.. _completedRanges.Where(r => r.End > currentTruncate)];

                // If all ranges are before truncate, clear the file
                if (snapshot.Count == 0)
                {
                    _rangesModified = false;

                    // Clear the persistence file since no ranges need to be saved
                    var filePath = GetRangesFilePath();
                    if (File.Exists(filePath))
                    {
                        try
                        {
                            File.Delete(filePath);
                            Logger.LogDebug("Deleted completed ranges file (all ranges before truncate) for type '{TypeName}'",
                                TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)));
                        }
                        catch (Exception ex)
                        {
                            Logger.LogWarning(ex, "Failed to delete completed ranges file for type '{TypeName}': {Message}",
                                TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)), ex.Message);
                        }
                    }
                    return;
                }

                // Warn if we're persisting too many ranges
                if (snapshot.Count > Configuration.MaxCompletedRanges * 0.8)
                {
                    Logger.LogWarning(
                        "Persisting {Count} ranges (approaching limit of {Max}) for type '{TypeName}'. " +
                        "This may indicate a persistent gap preventing progress.",
                        snapshot.Count,
                        Configuration.MaxCompletedRanges,
                        TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)));
                }
            }

            try
            {
                var filePath = GetRangesFilePath();

                // Ensure directory exists
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Serialize to JSON
                var json = System.Text.Json.JsonSerializer.Serialize(snapshot, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = false // Compact format for performance
                });

                // Write to file atomically by writing to temp file first
                var tempFilePath = filePath + ".tmp";

                // Use FileStream for .NET Standard 2.0 compatibility
                using (var stream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
                using (var writer = new StreamWriter(stream, System.Text.Encoding.UTF8))
                {
                    await writer.WriteAsync(json);
                    await writer.FlushAsync();
                }

                // Atomic rename (on most filesystems)
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                File.Move(tempFilePath, filePath);

                _rangesModified = false;
                _lastPersistTime = DateTime.UtcNow;

                Logger.LogDebug("Persisted {Count} completed ranges to {Path} for type '{TypeName}'",
                    snapshot.Count, filePath, TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to persist completed ranges for type '{TypeName}': {Message}",
                    TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)), ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Loads persisted completed ranges from disk.
        /// </summary>
        private void LoadCompletedRanges()
        {
            var filePath = GetRangesFilePath();

            if (!File.Exists(filePath))
            {
                Logger.LogInformation("No persisted ranges file found at {Path} for type '{TypeName}'",
                    filePath, TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)));
                return;
            }

            try
            {
                var json = File.ReadAllText(filePath);
                var ranges = System.Text.Json.JsonSerializer.Deserialize<List<CompletedRange>>(json);

                if (ranges == null || ranges.Count == 0)
                {
                    Logger.LogWarning("Persisted ranges file is empty at {Path} for type '{TypeName}'",
                        filePath, TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)));
                    return;
                }

                lock (_completedRangesLock)
                {
                    foreach (var range in ranges)
                    {
                        _completedRanges.Add(range);
                    }
                }

                Logger.LogInformation("Loaded {Count} persisted completed ranges from {Path} for type '{TypeName}'",
                    ranges.Count, filePath, TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to load persisted ranges from {Path} for type '{TypeName}': {Message}",
                    filePath, TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)), ex.Message);
                // Don't throw - continue with empty ranges rather than failing to start
            }
        }

        /// <summary>
        /// Gets the file path for persisting completed ranges.
        /// Each logger type has its own independent persistence file.
        /// </summary>
        private string GetRangesFilePath()
        {
            var directory = Path.Combine(
                Options.RootPath,
                TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)));

            return Path.Combine(directory, "completed_ranges.json");
        }

        /// <summary>
        /// Disposes of the logger resources.
        /// </summary>
        public void Dispose()
        {
            // Prevent multiple dispose calls
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            // Signal cancellation to all background tasks
            _disposeCts?.Cancel();

            // Complete the channel to signal no more writes
            // This allows the scan task to finish gracefully
            _pendingChannel?.Writer.Complete();

            // Final persist before shutdown if enabled and有 has changes
            if (Configuration.EnableRangePersistence && _rangesModified)
            {
                try
                {
                    PersistCompletedRangesAsync().GetAwaiter().GetResult();
                    Logger.LogInformation("Final persist completed successfully for type '{TypeName}'.",
                        TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)));
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed to perform final persist for type '{TypeName}': {Message}",
                        TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)), ex.Message);
                }
            }

            // Wait for all background tasks to complete gracefully with timeout
            if (_backgroundTasks.Count > 0)
            {
                try
                {
                    // Wait up to 5 seconds for graceful shutdown
                    Task.WaitAll(_backgroundTasks.ToArray(), TimeSpan.FromSeconds(5));
                    Logger.LogInformation("All background tasks completed gracefully for type '{TypeName}'.",
                        TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)));
                }
                catch (AggregateException ex)
                {
                    // Tasks were cancelled or failed - this is expected during shutdown
                    Logger.LogDebug("Background tasks completed with exceptions during shutdown for type '{TypeName}': {Message}",
                        TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)),
                        ex.Message);
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex, "Error while waiting for background tasks to complete for type '{TypeName}': {Message}",
                        TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)),
                        ex.Message);
                }
            }

            // Dispose resources
            Log?.Dispose();
            Iter?.Dispose();
            _commitSemaphore?.Dispose();
            _initializeSemaphore?.Dispose();
            _linkedCts?.Dispose();
            _disposeCts?.Dispose();
        }
    }

    /// <summary>
    /// Represents a completed range of log addresses.
    /// Wraps a Position to provide immutable range semantics for tracking completed entries.
    /// </summary>
    internal class CompletedRange : IAddressRange, IComparable<CompletedRange>
    {
        public long Start { get; }
        public long End { get; }

        /// <summary>
        /// Creates a completed range from a position.
        /// </summary>
        public CompletedRange(Position position)
        {
            if (position == null)
                throw new ArgumentNullException(nameof(position));

            Start = position.Address;
            End = position.NextAddress;
        }

        /// <summary>
        /// Creates a completed range from explicit start and end addresses.
        /// </summary>
        public CompletedRange(long start, long end)
        {
            if (start < 0)
            {
                throw new ArgumentException($"Start must be non-negative. Got: {start}", nameof(start));
            }

            if (end <= start)
            {
                throw new ArgumentException(
                    $"End must be greater than start. Start: {start}, End: {end}",
                    nameof(end));
            }

            Start = start;
            End = end;
        }

        public int CompareTo(CompletedRange? other)
        {
            if (other == null) return 1;

            // Sort by start address
            int startComparison = Start.CompareTo(other.Start);
            if (startComparison != 0)
                return startComparison;

            return End.CompareTo(other.End);
        }

        public override string ToString()
        {
            return $"[{Start}, {End})";
        }
    }
}
