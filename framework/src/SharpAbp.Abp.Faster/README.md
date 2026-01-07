# SharpAbp.Abp.Faster

A high-performance logging module built on [Microsoft FASTER](https://github.com/microsoft/FASTER) for ABP framework, providing efficient append-only log storage with support for multi-threaded concurrent consumption and automatic gap handling.

## Features

- ✅ **High-Performance Logging**: Built on Microsoft FASTER for efficient log operations
- ✅ **Multi-Threaded Concurrent Consumption**: Safe concurrent reads and out-of-order commits
- ✅ **Automatic Gap Handling**: Self-healing gaps with configurable timeout (default: 2 minutes)
- ✅ **Idempotent-Friendly Design**: Designed for business-layer idempotency patterns
- ✅ **Monitoring Metrics**: Built-in observability with performance counters
- ✅ **Resource Management**: Automatic cleanup and graceful shutdown
- ✅ **Memory Protection**: Configurable limits to prevent unbounded memory growth
- ✅ **Zero Configuration**: Works out-of-the-box with sensible defaults

## Architecture

### Core Components

#### Gap-Based Range Tracking
The module uses an interval merging algorithm to handle out-of-order commits:
- Tracks completed address ranges using `SortedSet<CompletedRange>`
- Merges continuous ranges to find the largest committable segment
- Automatically skips persistent gaps after timeout (default: 2 minutes)

#### Background Tasks
Four background tasks handle different aspects of log management:
1. **Commit Task**: Periodically commits written data (default: 2 seconds)
2. **Scan Task**: Reads data from FASTER log iterator into a buffered channel
3. **Complete Task**: Merges completed ranges and advances truncate address (default: 3 seconds)
4. **Truncate Task**: Removes old log segments that have been processed (default: 5 minutes)

## Quick Start

### Installation

```bash
dotnet add package SharpAbp.Abp.Faster
```

### Basic Configuration

```csharp
public class YourModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpFasterOptions>(options =>
        {
            options.RootPath = "/data/faster-logs";

            options.Configurations.Configure("default", c =>
            {
                c.FileName = "app.log";
                // That's it! Works with defaults for multi-threaded consumption
            });
        });
    }
}
```

### Basic Usage

```csharp
public class MyService
{
    private readonly IFasterLogger<MyData> _logger;

    public MyService(IFasterLoggerFactory loggerFactory)
    {
        _logger = loggerFactory.GetOrCreate<MyData>("default");
    }

    // Multi-threaded consumption example
    public void StartConsumers(int concurrency = 3)
    {
        for (var i = 0; i < concurrency; i++)
        {
            Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    // Safe concurrent reads
                    var entries = await _logger.ReadAsync(50, cancellationToken);

                    // Process data (implement idempotent logic)
                    foreach (var entry in entries)
                    {
                        if (await IsAlreadyProcessed(entry.Data.Id))
                            continue;

                        await ProcessData(entry.Data);
                        await MarkAsProcessed(entry.Data.Id);
                    }

                    // Out-of-order commits are handled automatically
                    await _logger.CommitAsync(entries.GetPositions(), cancellationToken);
                }
            });
        }
    }
}
```

## Configuration

### Default Configuration

The module works out-of-the-box with sensible defaults optimized for multi-threaded concurrent consumption:

```json
{
  "CommitIntervalMillis": 2000,              // Commit log data every 2 seconds
  "CompleteIntervalMillis": 3000,            // Merge ranges every 3 seconds
  "TruncateIntervalMillis": 300000,          // Truncate old data every 5 minutes
  "GapTimeoutMillis": 600000,                // Warn about gaps after 10 minutes
  "ForceCompleteGapTimeoutMillis": 120000,   // Auto-skip gaps after 2 minutes ✅
  "MaxCompletedRanges": 10000,               // Memory protection limit
  "PreReadCapacity": 5000                    // Buffered channel capacity
}
```

### Configuration Options

#### FasterOptions (Global)
| Property | Type | Description |
|----------|------|-------------|
| `RootPath` | `string` | Root directory for storing FASTER log files (required) |
| `Configurations` | `Dictionary` | Named configurations for different log instances |

#### Logger Configuration Properties

##### Core Settings
| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `FileName` | `string` | - | Name of the log file (required) |
| `IteratorName` | `string` | `"default"` | Unique identifier for the log iterator |
| `PreallocateFile` | `bool` | `false` | Preallocate the log file for better performance |
| `Capacity` | `long` | `4GB` | Maximum capacity of the log file in bytes |
| `RecoverDevice` | `bool` | `true` | Recover device state on startup |

##### Performance Settings
| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `PageSizeBits` | `int` | `22` | Page size (2^22 = 4MB) |
| `MemorySizeBits` | `int` | `23` | Memory size (2^23 = 8MB) |
| `SegmentSizeBits` | `int` | `30` | Segment size (2^30 = 1GB) |
| `PreReadCapacity` | `int` | `5000` | Channel capacity for pre-reading entries |
| `UseIoCompletionPort` | `bool` | `false` | Use I/O completion ports (Windows only) |
| `DisableFileBuffering` | `bool` | `true` | Disable OS file buffering for direct I/O |

##### Interval Settings
| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `CommitIntervalMillis` | `int` | `2000` | Interval for committing written data to FASTER log |
| `CompleteIntervalMillis` | `int` | `3000` | Interval for processing completed ranges |
| `TruncateIntervalMillis` | `int` | `300000` | Interval for truncating old log segments (5 min) |

##### Gap Handling Settings
| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `AddressMatchTolerance` | `long` | `10` | Bytes tolerance for considering ranges continuous |
| `GapTimeoutMillis` | `int` | `600000` | Timeout for warning about persistent gaps (10 min) |
| `ForceCompleteGapTimeoutMillis` | `int` | `120000` | **Auto-skip gap timeout (2 min, 0=disabled)** |
| `MaxCompletedRanges` | `int` | `10000` | Maximum number of ranges to track (memory protection) |

### Advanced Configuration Examples

#### High Reliability (Faster Gap Resolution)
```json
{
  "CommitIntervalMillis": 1000,
  "CompleteIntervalMillis": 1000,
  "TruncateIntervalMillis": 60000,
  "ForceCompleteGapTimeoutMillis": 30000   // 30 seconds
}
```

#### High Throughput (Tolerate Longer Gaps)
```json
{
  "CommitIntervalMillis": 5000,
  "CompleteIntervalMillis": 5000,
  "TruncateIntervalMillis": 600000,
  "ForceCompleteGapTimeoutMillis": 300000  // 5 minutes
}
```

#### Disable Auto-Skip (Manual Intervention Only)
```json
{
  "ForceCompleteGapTimeoutMillis": 0  // ⚠️ Not recommended for multi-threaded consumption
}
```

## Usage Patterns

### Pattern 1: Multi-Threaded Concurrent Consumption (Recommended)

**Best for**: High throughput scenarios with idempotent processing

```csharp
public void StartConsumers(int concurrency = 3)
{
    for (var i = 0; i < concurrency; i++)
    {
        Task.Factory.StartNew(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var batch = await _logger.ReadAsync(50, cancellationToken);

                // Process concurrently (idempotent logic required)
                foreach (var entry in batch)
                {
                    if (await IsAlreadyProcessed(entry.Data.Id))
                        continue;

                    await ProcessData(entry.Data);
                    await MarkAsProcessed(entry.Data.Id);
                }

                // Out-of-order commits handled automatically
                await _logger.CommitAsync(batch.GetPositions(), cancellationToken);

                await Task.Delay(100, cancellationToken);
            }
        }, TaskCreationOptions.LongRunning);
    }
}
```

**Key Points**:
- ✅ Multiple threads call `ReadAsync()` concurrently
- ✅ Multiple threads call `CommitAsync()` concurrently (out-of-order)
- ✅ Gaps are automatically handled (2-minute default timeout)
- ⚠️ **Requires idempotent processing** to handle potential reprocessing

### Pattern 2: Single Consumer with Internal Parallelism

**Best for**: Simple scenarios requiring guaranteed ordering

```csharp
public void StartConsumer()
{
    Task.Factory.StartNew(async () =>
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var batch = await _logger.ReadAsync(100, cancellationToken);

            // Process items in parallel within the batch
            await Parallel.ForEachAsync(batch,
                new ParallelOptions { MaxDegreeOfParallelism = 3 },
                async (item, ct) =>
                {
                    await ProcessItem(item);
                });

            // Sequential commit (no gaps)
            await _logger.CommitAsync(batch.GetPositions(), cancellationToken);
        }
    }, TaskCreationOptions.LongRunning);
}
```

**Key Points**:
- ✅ Single thread calls `ReadAsync()` and `CommitAsync()`
- ✅ No out-of-order commits (no gaps)
- ✅ Parallelism within batch processing
- ✅ No idempotency required

### Pattern 3: Writing Data

```csharp
// Single write
var address = await _logger.WriteAsync(data);

// Batch write
var addresses = await _logger.BatchWriteAsync(dataList);
```

## Understanding Gaps and Recovery

### What Are Gaps?

When using multi-threaded consumption, commits arrive out-of-order, creating gaps:

```
Scenario: 3 concurrent consumers
  Thread 1: Reads [0, 100)    → Processing slow ⏱️
  Thread 2: Reads [100, 200)  → Completes fast ✅ Commits [100, 200)
  Thread 3: Reads [200, 300)  → Completes fast ✅ Commits [200, 300)

Completed Ranges: [100, 200), [200, 300)
Gap: [0, 100) ← Not yet committed

Progress: Can only truncate up to 0 (waiting for the gap to fill)
```

### Automatic Gap Handling

**Default Behavior** (ForceCompleteGapTimeoutMillis = 120000):

```
T+0:   Gap detected at address 0
T+120s: Gap still present
        → Auto-skip gap [0, 100)
        → Log ERROR with warning
        → Truncate advances to 300 ✅
        → System continues processing
```

**What Happens on Restart?**

```
Before Restart:
  Truncate Address: 0 (blocked by gap)
  FASTER Iterator: Stores CompletedUntilAddress = 0

After Restart:
  Resume from: 0
  Gap auto-skipped (if still present after 2 min)
  Result: [0, 100) may be reprocessed if gap eventually fills
          This is why idempotent processing is critical!
```

### Best Practices for Gap Handling

#### 1. Implement Idempotent Processing

**Critical for multi-threaded consumption!**

```csharp
public async Task ProcessEntry(LogEntry<MyData> entry)
{
    // Check if already processed
    if (await _db.ExistsAsync(entry.Data.Id))
    {
        _logger.LogDebug("Skipping duplicate: {Id}", entry.Data.Id);
        return;
    }

    // Process data
    await _db.InsertAsync(entry.Data);

    // Mark as processed (atomic operation)
    await _db.MarkProcessedAsync(entry.Data.Id, DateTime.UtcNow);
}
```

#### 2. Monitor Gaps

```csharp
// Periodic monitoring
if (_logger.CurrentGapCount > 10)
{
    _logger.LogWarning(
        "High gap count: {Count}, Largest: {Size} bytes",
        _logger.CurrentGapCount,
        _logger.LargestGapSize);
}
```

#### 3. Tune Gap Timeout Based on Workload

```json
// Fast processing (< 30s per batch)
{
  "ForceCompleteGapTimeoutMillis": 60000  // 1 minute
}

// Slow processing (> 1 minute per batch)
{
  "ForceCompleteGapTimeoutMillis": 300000  // 5 minutes
}

// Very slow or unreliable processing
{
  "ForceCompleteGapTimeoutMillis": 600000  // 10 minutes
}
```

#### 4. Handle Stuck Consumers

If a consumer thread crashes or hangs:
- Gap persists indefinitely without auto-skip
- Auto-skip kicks in after timeout
- Data in gap range is skipped (potential data loss)
- **Solution**: Monitor consumer health and restart stuck threads

## Monitoring

### Built-in Metrics

```csharp
public void DisplayMetrics()
{
    _logger.LogInformation(
        "FASTER Metrics - " +
        "Writes: {Writes}, Reads: {Reads}, Committed: {Committed}, " +
        "Gaps: {Gaps}, LargestGap: {LargestGap} bytes, " +
        "Ranges: {Ranges}, Truncate: {Truncate}",
        _logger.TotalWriteCount,
        _logger.TotalReadCount,
        _logger.TotalCommittedRanges,
        _logger.CurrentGapCount,
        _logger.LargestGapSize,
        _logger.CompletedRangeCount,
        _logger.TruncateBeforeAddress
    );
}
```

### Key Metrics

| Metric | Property | Alert Threshold |
|--------|----------|-----------------|
| Gap Count | `CurrentGapCount` | > 10: Check consumer health |
| Largest Gap Size | `LargestGapSize` | > 1MB: Investigate stuck consumer |
| Completed Ranges | `CompletedRangeCount` | > 1000: Many persistent gaps |
| Truncate Address | `TruncateBeforeAddress` | Not advancing: Progress blocked |

## API Reference

### IFasterLogger\<T\>

#### Properties
- `bool Initialized` - Whether the logger has been initialized
- `long TotalCommittedRanges` - Total number of committed ranges
- `long TotalWriteCount` - Total number of writes
- `long TotalReadCount` - Total number of reads
- `long CurrentGapCount` - Current number of gaps in completed ranges
- `long LargestGapSize` - Largest gap size detected (bytes)
- `int CompletedRangeCount` - Number of completed ranges tracked
- `long TruncateBeforeAddress` - Current truncate address

#### Methods
- `void Initialize()` - Initialize the logger
- `Task<long> WriteAsync(T entity, CancellationToken)` - Write single entry
- `Task<List<long>> BatchWriteAsync(List<T> values, CancellationToken)` - Write batch
- `Task<LogEntryList<T>> ReadAsync(int count, CancellationToken)` - Read entries
- `Task CommitAsync(IEnumerable<Position> positions, CancellationToken)` - Commit positions
- `Task ForceCommitGap(long gapStart, long gapEnd, CancellationToken)` - Manually skip a gap (⚠️ data loss!)

### Position

```csharp
public class Position : IAddressRange
{
    public long Address { get; set; }        // Start address
    public long NextAddress { get; set; }    // End address (exclusive)
    public long Length => NextAddress - Address;

    public bool IsValid() => Address >= 0 && NextAddress > Address;
}
```

### LogEntry\<T\>

```csharp
public class LogEntry<T> : IAddressRange
{
    public T? Data { get; set; }
    public long CurrentAddress { get; set; }
    public long NextAddress { get; set; }
    public long EntryLength => NextAddress - CurrentAddress;
}
```

## Troubleshooting

### Issue: Data Reprocessing After Restart

**Symptoms**: Same data processed multiple times after restart

**Cause**:
- Gap auto-skipped while consumer was processing
- Restart happens before gap fills
- Data reprocessed when gap eventually fills

**Solution**:
```csharp
// Implement idempotent processing (REQUIRED for multi-threaded consumption)
if (await IsAlreadyProcessed(entry.Data.Id))
{
    await _logger.CommitAsync(entry.GetPositions());
    return;
}
await ProcessData(entry.Data);
await MarkAsProcessed(entry.Data.Id);
await _logger.CommitAsync(entry.GetPositions());
```

### Issue: Gaps Not Clearing

**Symptoms**: `CurrentGapCount` stays high

**Possible Causes**:
1. Consumer thread crashed/hung
2. Processing taking longer than gap timeout
3. Commit logic not being called

**Solutions**:
- Monitor consumer thread health
- Increase `ForceCompleteGapTimeoutMillis`
- Ensure all reads are followed by commits
- Check for exceptions in processing logic

### Issue: High Memory Usage

**Symptoms**: Memory grows unbounded

**Causes**:
1. Too many gaps (ranges not merging)
2. `PreReadCapacity` too large
3. `MaxCompletedRanges` too high

**Solutions**:
```json
{
  "MaxCompletedRanges": 1000,      // Reduce from 10000
  "PreReadCapacity": 1000,         // Reduce from 5000
  "ForceCompleteGapTimeoutMillis": 60000  // Reduce timeout
}
```

### Issue: Progress Not Advancing

**Symptoms**: `TruncateBeforeAddress` not increasing

**Diagnosis**:
```csharp
// Check gap metrics
_logger.LogWarning(
    "Progress stuck - Gaps: {Gaps}, Truncate: {Truncate}",
    _logger.CurrentGapCount,
    _logger.TruncateBeforeAddress
);
```

**Solutions**:
1. Check if gaps are being auto-skipped (look for ERROR logs)
2. Verify `ForceCompleteGapTimeoutMillis` is not 0
3. Manually force complete if needed:
   ```csharp
   await _logger.ForceCommitGap(gapStart, gapEnd);
   ```

## Version History

### v2.5 - Simplified Architecture (Current)
- ✅ **Removed JSON persistence** (unnecessary with auto-gap handling)
- ✅ **Default enabled auto-gap skip** (120s timeout)
- ✅ **Optimized for multi-threaded consumption**
- ✅ Reduced from 5 to 4 background tasks
- ✅ Simplified configuration (removed persistence settings)
- ✅ Improved documentation with idempotency patterns
- ⚠️ **Breaking**: Removed `EnableRangePersistence` and `PersistIntervalMillis` configs

### v2.4 - Force Complete Mechanism
- ✅ Implemented auto gap skipping with timeout
- ✅ Manual gap filling API
- ✅ Enhanced gap lifecycle tracking

### v2.3 - Range Persistence
- ✅ JSON persistence to prevent reprocessing (now removed in v2.5)

### v2.0-2.2 - Core Features
- ✅ Gap-based architecture
- ✅ Memory protection
- ✅ Resource management
- ✅ Validation & safety

## Migration Guide (v2.4 → v2.5)

### Removed Configurations

```json
// BEFORE (v2.4)
{
  "EnableRangePersistence": true,
  "PersistIntervalMillis": 30000,
  "ForceCompleteGapTimeoutMillis": 0
}

// AFTER (v2.5)
{
  "ForceCompleteGapTimeoutMillis": 120000  // Now default enabled!
  // persistence configs removed
}
```

### Required Code Changes

**None!** The module still works the same way. However:

⚠️ **Action Required**: Implement idempotent processing if using multi-threaded consumption:

```csharp
// Add idempotency check
if (await IsAlreadyProcessed(entry.Data.Id))
    return;

await ProcessData(entry.Data);
await MarkAsProcessed(entry.Data.Id);
```

## License

This module is part of the SharpAbp framework.

## Contributing

Contributions are welcome! Please ensure:
- Thread safety in concurrent scenarios
- Proper resource disposal
- Comprehensive error handling
- Unit tests for new features
