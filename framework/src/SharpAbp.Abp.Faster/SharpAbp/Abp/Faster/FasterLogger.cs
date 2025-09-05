using FASTER.core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
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
        private long _completedUntilAddress = -1L;
        private long _truncateUntilAddress = -1L;
        private bool _initialized = false;
        private readonly ConcurrentDictionary<long, RetryPosition> _committing;
        protected Channel<BufferedLogEntry> _pendingChannel;

        protected ILogger Logger { get; }
        protected AbpFasterOptions Options { get; }
        protected AbpFasterConfiguration Configuration { get; }
        protected IObjectSerializer Serializer { get; }
        protected ICancellationTokenProvider CancellationTokenProvider { get; }
        protected FasterLog? Log { get; set; }
        protected FasterLogScanIterator? Iter { get; set; }

        /// <summary>
        /// Gets whether the logger has been initialized.
        /// </summary>
        public bool Initialized => _initialized;

        /// <summary>
        /// Gets the name of the logger.
        /// </summary>
        public string? Name { get; }

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

            _pendingChannel = Channel.CreateBounded<BufferedLogEntry>(Configuration.PreReadCapacity);
            _committing = new ConcurrentDictionary<long, RetryPosition>();
        }

        /// <summary>
        /// Initializes the FASTER logger with the specified configuration.
        /// </summary>
        public virtual void Initialize()
        {
            if (_initialized)
            {
                Logger.LogWarning(
                    "FASTER logger '{LoggerName}' is already initialized.",
                    TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)));
                return;
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

            _completedUntilAddress = Math.Max(Iter.CompletedUntilAddress, Iter.BeginAddress);

            // Start background tasks
            StartScheduleCommit();
            StartScheduleScan();
            StartScheduleComplete();
            StartScheduleTruncate();

            _initialized = true;

            Logger.LogInformation(
                "FASTER logger '{LoggerName}' initialized successfully. Begin address: {BeginAddress}, Completed address: {CompletedAddress}, File: {FileName}.",
                TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)),
                Iter.BeginAddress,
                _completedUntilAddress,
                fileName);
        }

        /// <summary>
        /// Starts the scheduled commit task to periodically commit written data.
        /// This prevents data loss due to unexpected shutdowns.
        /// </summary>
        protected virtual void StartScheduleCommit()
        {
            Task.Factory.StartNew(async () =>
            {
                while (!CancellationTokenProvider.Token.IsCancellationRequested)
                {
                    await Task.Delay(Configuration.CommitIntervalMillis, CancellationTokenProvider.Token);
                    try
                    {
                        await _commitSemaphore.WaitAsync(CancellationTokenProvider.Token);
                        if (Log != null)
                        {
                            await Log.CommitAsync(CancellationTokenProvider.Token);
                        }
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
                }
            }, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Starts the scheduled scan task to read data from the log iterator.
        /// </summary>
        protected virtual void StartScheduleScan()
        {
            Task.Factory.StartNew(async () =>
            {
                while (!CancellationTokenProvider.Token.IsCancellationRequested)
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
                                await Iter.WaitAsync(CancellationTokenProvider.Token);
                            }
                        }

                        Logger.LogTrace(
                            "Retrieved iterator message for type '{TypeName}'. Current address: {CurrentAddress}, Next address: {NextAddress}.",
                            TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)),
                            currentAddress,
                            nextAddress);

                        if (buffer != null && entryLength > 0)
                        {
                            var entry = new BufferedLogEntry
                            {
                                Data = buffer,
                                EntryLength = entryLength,
                                CurrentAddress = currentAddress,
                                NextAddress = nextAddress
                            };

                            await _pendingChannel.Writer.WriteAsync(entry, CancellationTokenProvider.Token);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Error during scheduled log scan for type '{TypeName}': {Message}", 
                            TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)), ex.Message);
                    }
                }
            }, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Starts the scheduled completion task to process completed log entries.
        /// </summary>
        protected virtual void StartScheduleComplete()
        {
            Task.Factory.StartNew(async () =>
            {
                while (!CancellationTokenProvider.Token.IsCancellationRequested)
                {
                    try
                    {
                        long commitAddress = _completedUntilAddress;
                        var removeIds = new List<long>();

                        // Pre-sort _committing to avoid repeated sorting in the loop
                        var sortedCommits = _committing.Values.OrderBy(x => x.Position!.Address).ToList();
                        Logger.LogDebug("Processing {Count} pending commit operations for type '{TypeName}'.", 
                            _committing.Count, TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)));

                        foreach (var commit in sortedCommits)
                        {
                            if (commit.Position != null && !commit.Position.IsMatch(commitAddress))
                            {
                                Logger.LogDebug(
                                    "Commit position mismatch for type '{TypeName}'. Expected: {ExpectedAddress}, Actual - Address: {Address}, Next: {NextAddress}",
                                    TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)),
                                    commitAddress,
                                    commit.Position.Address,
                                    commit.Position.NextAddress);

                                if (commit.IsMax(Configuration.MaxCommitSkip))
                                {
                                    // If max retry count reached, update commitAddress and log
                                    if (Iter != null && commitAddress <= Iter.EndAddress)
                                    {
                                        commitAddress = commit.Position.NextAddress;
                                        removeIds.Add(commit.Position.Address);
                                        Logger.LogDebug(
                                            "Maximum commit skip count reached for address {Address} in type '{TypeName}'. Proceeding to next.",
                                            commit.Position.Address,
                                            TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)));
                                    }
                                }
                                else
                                {
                                    // Increment retry count and update dictionary
                                    var gainPosition = new RetryPosition(commit.Position, commit.RetryCount + 1);
                                    if (!_committing.TryUpdate(commit.Position.Address, gainPosition, commit))
                                    {
                                        Logger.LogWarning(
                                            "Failed to update retry position for address {Address} in type '{TypeName}'.",
                                            commit.Position.Address,
                                            TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)));
                                    }
                                    break; // Exit loop to wait for next schedule
                                }
                            }
                            else
                            {
                                if (Iter != null && commitAddress <= Iter.EndAddress && commit.Position != null)
                                {
                                    commitAddress = commit.Position.NextAddress;
                                    removeIds.Add(commit.Position.Address);
                                }
                            }
                        }

                        // Check if we need to commit completed records
                        if (commitAddress > _completedUntilAddress &&
                            Iter != null &&
                            commitAddress <= Iter.EndAddress)
                        {
                            await CompleteUntilRecordAtAsync(commitAddress);
                            _completedUntilAddress = commitAddress;
                            RemoveProcessedCommits(removeIds);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Error during commit completion for type '{TypeName}': {Message}", 
                            TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)), ex.Message);
                    }

                    await Task.Delay(Configuration.CompleteIntervalMillis, CancellationTokenProvider.Token);
                }
            }, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Completes records up to the specified address.
        /// </summary>
        /// <param name="commitAddress">The address up to which records should be completed.</param>
        private async Task CompleteUntilRecordAtAsync(long commitAddress)
        {
            Logger.LogDebug("Completing records up to address {CommitAddress} for type '{TypeName}'.", 
                commitAddress, TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)));
            await _commitSemaphore.WaitAsync(CancellationTokenProvider.Token);
            try
            {
                if (Log != null)
                {
                    await Log.CommitAsync(CancellationTokenProvider.Token);
                }

                if (Iter != null)
                {
                    await Iter.CompleteUntilRecordAtAsync(commitAddress, CancellationTokenProvider.Token);
                }
            }
            finally
            {
                _commitSemaphore.Release();
            }
        }

        /// <summary>
        /// Removes processed commit entries from the committing dictionary.
        /// </summary>
        /// <param name="removeIds">The list of IDs to remove.</param>
        private void RemoveProcessedCommits(List<long> removeIds)
        {
            foreach (var id in removeIds)
            {
                if (!_committing.TryRemove(id, out _))
                {
                    Logger.LogWarning("Failed to remove commit entry with ID {Id} for type '{TypeName}'.", 
                        id, TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)));
                }
            }
        }

        /// <summary>
        /// Starts the scheduled truncation task to remove completed log segments.
        /// </summary>
        protected virtual void StartScheduleTruncate()
        {
            Task.Factory.StartNew(async () =>
            {
                while (!CancellationTokenProvider.Token.IsCancellationRequested)
                {
                    try
                    {
                        if (_truncateUntilAddress < _completedUntilAddress && Log != null)
                        {
                            Log.TruncateUntilPageStart(_completedUntilAddress);
                            Logger.LogDebug(
                                "Truncated log until page start. Address: {TruncateAddress} for type '{TypeName}'.",
                                _completedUntilAddress,
                                TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)));
                            _truncateUntilAddress = _completedUntilAddress;
                        }
                        else
                        {
                            Logger.LogDebug(
                                "Skipping truncation for type '{TypeName}'. Truncate address ({TruncateAddress}) >= completed address ({CompletedAddress}).",
                                TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)),
                                _truncateUntilAddress,
                                _completedUntilAddress);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Error during log truncation for type '{TypeName}': {Message}", 
                            TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)), ex.Message);
                    }

                    await Task.Delay(Configuration.TruncateIntervalMillis, CancellationTokenProvider.Token);
                }
            }, TaskCreationOptions.LongRunning);
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

            return await Log.EnqueueAsync(buffer, cancellationToken);
        }

        /// <summary>
        /// Writes a batch of entities to the log.
        /// </summary>
        /// <param name="values">The entities to write.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The positions of the written entries.</returns>
        public virtual async Task<List<long>> BatchWriteAsync(List<T> values, CancellationToken cancellationToken = default)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            var positions = new List<long>();
            if (Log == null)
            {
                throw new InvalidOperationException("Logger has not been initialized.");
            }

            foreach (var entity in values)
            {
                var buffer = Serializer.Serialize(entity);
                var position = await Log.EnqueueAsync(buffer, cancellationToken);
                positions.Add(position);
            }

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
                        EntryLength = entry.EntryLength,
                        NextAddress = entry.NextAddress,
                    });
                }
            }

            return entries;
        }

        /// <summary>
        /// Commits the specified log entry positions.
        /// </summary>
        /// <param name="entryPosition">The positions to commit.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public virtual Task CommitAsync(LogEntryPosition entryPosition, CancellationToken cancellationToken = default)
        {
            if (entryPosition == null)
            {
                throw new ArgumentNullException(nameof(entryPosition));
            }

            Logger.LogDebug("Committing {Count} positions for type '{TypeName}'.", 
                entryPosition.Count, TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)));

            foreach (var position in entryPosition)
            {
                if (!_committing.TryAdd(position.Address, new RetryPosition(position, 0)))
                {
                    Logger.LogDebug("Failed to add position {Address} to committing dictionary for type '{TypeName}'.", 
                        position.Address, TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)));
                }
            }

            Logger.LogDebug("Finished committing positions for type '{TypeName}'.", 
                TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(T)));

            return Task.CompletedTask;
        }

        /// <summary>
        /// Disposes of the logger resources.
        /// </summary>
        public void Dispose()
        {
            Log?.Dispose();
            Iter?.Dispose();
            _commitSemaphore?.Dispose();
        }
    }
}
