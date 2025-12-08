# SharpAbp.Abp.Faster

A high-performance logging module built on [Microsoft FASTER](https://github.com/microsoft/FASTER) for ABP framework, providing efficient append-only log storage with support for multi-threaded writes and out-of-order commits.

## Features

- ✅ **High-Performance Logging**: Built on Microsoft FASTER for efficient log operations
- ✅ **Multi-Threaded Support**: Safe concurrent writes and reads
- ✅ **Out-of-Order Commit**: Handles non-sequential commit scenarios using gap-based interval tracking
- ✅ **Gap Handling**: Automatic and manual mechanisms to skip persistent gaps (configurable)
- ✅ **Monitoring Metrics**: Built-in observability with performance counters
- ✅ **Resource Management**: Automatic cleanup and graceful shutdown
- ✅ **Memory Protection**: Configurable limits to prevent unbounded memory growth
- ✅ **Range Persistence**: Anti-reprocessing mechanism to prevent data loss on restart

## Architecture

### Core Components

#### Gap-Based Range Tracking
The module uses an interval merging algorithm to handle out-of-order commits:
- Tracks completed address ranges using `SortedSet<CompletedRange>`
- Merges continuous ranges to find the largest committable segment
- Supports configurable tolerance for small gaps

#### Background Tasks
Four background tasks handle different aspects of log management:
1. **Commit Task**: Periodically commits written data to prevent data loss
2. **Scan Task**: Reads data from FASTER log iterator into a buffered channel
3. **Complete Task**: Merges completed ranges and advances truncate address
4. **Truncate Task**: Removes old log segments that have been processed

## Configuration

### Basic Configuration

Configure in `appsettings.json`:

```json
{
  "FasterOptions": {
    "RootPath": "/data/faster-logs",
    "Configurations": {
      "default": {
        "FileName": "app.log",
        "PreallocateFile": true,
        "Capacity": "4294967296",
        "RecoverDevice": true,
        "UseIoCompletionPort": false,
        "DisableFileBuffering": true,
        "ScanUncommitted": false,
        "AutoRefreshSafeTailAddress": false,
        "CommitIntervalMillis": 1000,
        "CompleteIntervalMillis": 1000,
        "TruncateIntervalMillis": 300000,
        "PreReadCapacity": 5000,
        "AddressMatchTolerance": 0,
        "GapTimeoutMillis": 600000,
        "ForceCompleteGapTimeoutMillis": 0,
        "MaxCompletedRanges": 10000,
        "EnableRangePersistence": true,
        "PersistIntervalMillis": 30000,
        "IteratorName": "default-iter"
      }
    }
  }
}
```

### Configuration Options

#### FasterOptions
| Property | Type | Description |
|----------|------|-------------|
| `RootPath` | `string` | Root directory for storing FASTER log files (required) |
| `Configurations` | `Dictionary` | Named configurations for different log instances |

#### Logger Configuration Properties

##### Core Settings
| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `FileName` | `string` | - | Name of the log file (required) |
| `PreallocateFile` | `bool` | `false` | Preallocate the log file for better performance |
| `Capacity` | `long` | - | Maximum capacity of the log file in bytes |
| `RecoverDevice` | `bool` | `false` | Recover device state on startup |
| `IteratorName` | `string` | - | Unique identifier for the log iterator |

##### Performance Settings
| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `UseIoCompletionPort` | `bool` | `false` | Use I/O completion ports (Windows only) |
| `DisableFileBuffering` | `bool` | `false` | Disable OS file buffering for direct I/O |
| `ScanUncommitted` | `bool` | `false` | Allow scanning uncommitted entries |
| `AutoRefreshSafeTailAddress` | `bool` | `false` | Automatically refresh safe tail address |
| `PreReadCapacity` | `int` | `1000` | Channel capacity for pre-reading entries |

##### Interval Settings
| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `CommitIntervalMillis` | `int` | `1000` | Interval for committing written data (ms) |
| `CompleteIntervalMillis` | `int` | `1000` | Interval for processing completed ranges (ms) |
| `TruncateIntervalMillis` | `int` | `300000` | Interval for truncating old log segments (ms) |

##### Advanced Settings
| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `AddressMatchTolerance` | `long` | `0` | Bytes tolerance for considering ranges continuous |
| `GapTimeoutMillis` | `int` | `600000` | Timeout for warning about persistent gaps (10 min) |
| `ForceCompleteGapTimeoutMillis` | `int` | `0` | Auto-skip gap timeout (0=disabled, WARNING: data loss!) |
| `MaxCompletedRanges` | `int` | `10000` | Maximum number of ranges to track (memory protection) |
| `EnableRangePersistence` | `bool` | `true` | Enable persistence of completed ranges to disk |
| `PersistIntervalMillis` | `int` | `5000` | Interval for persisting completed ranges (5 sec) |
| `PageSizeBits` | `int` | `25` | Page size as power of 2 (default: 32MB) |
| `MemorySizeBits` | `int` | `26` | Memory size as power of 2 (default: 64MB) |
| `SegmentSizeBits` | `int` | `30` | Segment size as power of 2 (default: 1GB) |

## Usage

### Basic Usage

```csharp
public class MyService
{
    private readonly IFasterLogger<MyData> _logger;

    public MyService(IFasterLoggerFactory loggerFactory)
    {
        _logger = loggerFactory.GetOrCreate<MyData>("default");
    }

    public async Task LogDataAsync(MyData data)
    {
        // Write a single entry
        var address = await _logger.WriteAsync(data);

        // Commit the position
        var position = new Position(address, address + length);
        await _logger.CommitAsync(new[] { position });
    }
}
```

### Batch Operations

```csharp
public async Task LogBatchAsync(List<MyData> items)
{
    // Batch write
    var addresses = await _logger.BatchWriteAsync(items);

    // Create positions and commit
    var positions = addresses.Select((addr, i) =>
        new Position(addr, addr + itemLengths[i])).ToList();

    await _logger.CommitAsync(positions);
}
```

### Reading Entries

```csharp
public async Task<LogEntryList<MyData>> ReadEntriesAsync(int count = 10)
{
    // Read from the buffered channel
    var entries = await _logger.ReadAsync(count);

    // Process entries
    foreach (var entry in entries)
    {
        Console.WriteLine($"Address: {entry.CurrentAddress}, Data: {entry.Data}");
    }

    // Get positions for commit
    var positions = entries.GetPositions();
    await _logger.CommitAsync(positions);

    return entries;
}
```

### Monitoring

```csharp
public void DisplayMetrics()
{
    Console.WriteLine($"Total Writes: {_logger.TotalWriteCount}");
    Console.WriteLine($"Total Reads: {_logger.TotalReadCount}");
    Console.WriteLine($"Total Committed Ranges: {_logger.TotalCommittedRanges}");
    Console.WriteLine($"Current Gaps: {_logger.CurrentGapCount}");
    Console.WriteLine($"Largest Gap Size: {_logger.LargestGapSize} bytes");
    Console.WriteLine($"Truncate Address: {_logger.TruncateBeforeAddress}");
    Console.WriteLine($"Completed Range Count: {_logger.CompletedRangeCount}");
}
```

## Key Interfaces and Classes

### IFasterLogger\<T\>

Main interface for FASTER logger operations.

**Properties:**
- `bool Initialized` - Whether the logger has been initialized
- `long TotalCommittedRanges` - Total number of committed ranges
- `long TotalWriteCount` - Total number of writes
- `long TotalReadCount` - Total number of reads
- `long CurrentGapCount` - Current number of gaps in completed ranges
- `long LargestGapSize` - Largest gap size detected (bytes)
- `int CompletedRangeCount` - Number of completed ranges tracked
- `long TruncateBeforeAddress` - Current truncate address

**Methods:**
- `void Initialize()` - Initialize the logger
- `Task<long> WriteAsync(T entity, CancellationToken)` - Write single entry
- `Task<List<long>> BatchWriteAsync(List<T> values, CancellationToken)` - Write batch
- `Task<LogEntryList<T>> ReadAsync(int count, CancellationToken)` - Read entries
- `Task CommitAsync(IEnumerable<Position> positions, CancellationToken)` - Commit positions
- `Task ForceCommitGap(long gapStart, long gapEnd, CancellationToken)` - Manually skip a persistent gap (⚠️ data loss!)

### Position

Represents an address range in the log.

```csharp
public class Position : IAddressRange
{
    public long Address { get; set; }        // Start address
    public long NextAddress { get; set; }    // End address (exclusive)
    public long Length => NextAddress - Address;  // Computed length

    public bool IsValid() => Address >= 0 && NextAddress > Address;
}
```

### LogEntry\<T\>

Represents a log entry with data and address information.

```csharp
public class LogEntry<T> : IAddressRange
{
    public T? Data { get; set; }
    public long CurrentAddress { get; set; }
    public long NextAddress { get; set; }
    public long EntryLength => NextAddress - CurrentAddress;
}
```

## Advanced Topics

### Out-of-Order Commit Handling

The module is designed for scenarios where commits arrive out of order:

1. **Gap-Based Tracking**: Uses `SortedSet<CompletedRange>` to track completed ranges
2. **Interval Merging**: Merges continuous ranges to find the largest committable segment
3. **Tolerance Support**: `AddressMatchTolerance` allows treating small gaps as continuous
4. **Gap Timeout**: Warns when gaps persist longer than `GapTimeoutMillis`

### Range Persistence (Anti-Reprocessing)

**⚠️ IMPORTANT**: To prevent data reprocessing after restart, the module persists completed ranges to disk.

#### Why Persistence is Needed

FASTER's iterator can only commit **continuous** address ranges. If your application processes data out-of-order:

```
Scenario: Write addresses 0-999
- Process and commit: [0-100)   ✅
- Process and commit: [200-300) ✅
- Skip (not committed): [100-200) ❌  (gap!)
- Process and commit: [300-400) ✅

Result: Only [0-100) can be committed to FASTER due to the gap.
After restart: [100-999) will be reprocessed! ❌
```

#### Persistence Mechanism

The module solves this by persisting `_completedRanges` to disk:

1. **Automatic Persistence**: Ranges saved every `PersistIntervalMillis` (default: 5 seconds)
2. **Shutdown Persistence**: Final save on graceful shutdown
3. **Startup Recovery**: Loads persisted ranges on initialization
4. **Optimized Storage**: Only saves ranges beyond current truncate address
5. **Independent Files**: Each logger configuration has its own `completed_ranges.json`

**File Location**:
```
{RootPath}/
  {TypeName}/
    completed_ranges.json  ← Persisted ranges
    {FileName}             ← FASTER log data
```

#### Configuration

```json
{
  "EnableRangePersistence": true,    // Enable/disable persistence
  "PersistIntervalMillis": 5000      // Save interval (5 sec)
}
```

**Disable for Testing**:
```json
{
  "EnableRangePersistence": false    // Reprocess all data on restart
}
```

#### Data Loss Scenarios

| Scenario | Data Loss | Impact |
|----------|-----------|--------|
| **Normal Shutdown** | None | Ranges saved in Dispose |
| **Crash (5s interval)** | Up to 5 seconds | May reprocess recent data |
| **Crash (1s interval)** | Up to 1 second | Minimal reprocessing |
| **Persistence Disabled** | All uncommitted | Full reprocessing |

#### Best Practices

1. **Always Commit After Processing**:
   ```csharp
   var entries = await logger.ReadAsync(100);
   // Process entries...
   await logger.CommitAsync(entries.GetPositions());  // ✅ Don't forget!
   ```

2. **Handle Reprocessing**:
   ```csharp
   // Make processing idempotent
   if (await IsAlreadyProcessed(entry.Data.Id))
   {
       continue; // Skip duplicate
   }
   await ProcessData(entry.Data);
   await MarkAsProcessed(entry.Data.Id);
   ```

3. **Tune Persistence Interval**:
   ```csharp
   // High reliability (more I/O)
   "PersistIntervalMillis": 1000  // 1 second

   // Balanced (recommended)
   "PersistIntervalMillis": 5000  // 5 seconds

   // High performance (less reliable)
   "PersistIntervalMillis": 30000 // 30 seconds
   ```

4. **Monitor Range Growth**:
   ```csharp
   if (logger.CompletedRangeCount > 1000)
   {
       _logger.LogWarning("High number of gaps: {Count}", logger.CompletedRangeCount);
       // Investigate why ranges aren't being merged
   }
   ```

### Force Complete Mechanism (Gap Handling)

**⚠️ WARNING**: This feature allows skipping persistent gaps but **WILL CAUSE DATA LOSS** for entries in the gap range!

#### The Gap Problem

When commit data is lost or delayed indefinitely, gaps prevent progress:

```
Scenario: Commits arrive out of order
- Committed: [0-100)    ✅
- Committed: [200-300)  ✅ (waiting for [100-200) to arrive)
- Lost:      [100-200)  ❌ (this data will NEVER be committed)

Result: Progress stuck at 100, [200-300) and all subsequent ranges cannot be committed.
System continues running but no progress is made!
```

#### Solution 1: Automatic Force Complete (Time-Based)

The system can automatically skip gaps that persist beyond a configured timeout:

**Configuration**:
```json
{
  "ForceCompleteGapTimeoutMillis": 600000  // 10 minutes (0 = disabled)
}
```

**Behavior**:
- When a gap persists longer than the timeout, the system automatically skips it
- Logs an ERROR message with gap details
- Continues processing subsequent ranges
- ⚠️ Data in the gap range is LOST and will NOT be reprocessed

**Example Log Output**:
```
[ERROR] FORCING COMPLETION past gap at address 100 (size: 100 bytes)
        ⚠️ DATA IN RANGE [100, 200) WILL BE SKIPPED AND MAY BE LOST!
        Gap persisted for 00:10:05, exceeded timeout of 00:10:00
```

**When to Use**:
- You have idempotent processing and can tolerate small data loss
- Gap data is genuinely lost (e.g., source system confirmed deletion)
- Operational continuity is more important than complete data accuracy

**When NOT to Use**:
- Zero data loss requirement
- Gaps are temporary (processing delays, not lost data)
- You need manual verification before skipping

#### Solution 2: Manual Force Complete (Emergency Tool)

For emergency intervention, you can manually skip a specific gap:

```csharp
// WARNING: This will cause data loss for entries in [100, 200)!
await logger.ForceCommitGap(
    gapStart: 100,
    gapEnd: 200,
    cancellationToken: cancellationToken
);
```

**Use Cases**:
- Production incident requiring immediate resolution
- Gap confirmed as permanent data loss (e.g., source system failure)
- Automatic timeout not configured, but manual intervention needed

**Safety Checks**:
- Validates gap addresses (non-negative, end > start)
- Logs ERROR with gap details and data loss warning
- Only adds gap if not already in completed ranges
- Resets gap timeout tracking for that specific gap

#### Best Practices for Gap Handling

1. **Default: Disable Automatic Skip**:
   ```json
   {
     "ForceCompleteGapTimeoutMillis": 0  // Manual intervention required
   }
   ```

2. **Enable Only If Appropriate**:
   ```json
   {
     "ForceCompleteGapTimeoutMillis": 1800000  // 30 minutes
   }
   ```
   - Use longer timeouts (30+ minutes) to avoid premature data loss
   - Shorter timeouts = higher risk of skipping recoverable gaps

3. **Monitor Gap Warnings**:
   ```csharp
   // Set up alerts for gap timeout warnings (default: 10 minutes)
   if (logger.CurrentGapCount > 0)
   {
       _logger.LogWarning("Active gaps detected: {Count}, Largest: {Size} bytes",
           logger.CurrentGapCount,
           logger.LargestGapSize);
       // Alert operations team to investigate
   }
   ```

4. **Investigate Before Forcing**:
   - Check if processing threads are stuck or crashed
   - Verify source system hasn't lost the data
   - Confirm the gap isn't just a temporary delay

5. **Use Idempotent Processing**:
   ```csharp
   // Make processing idempotent to handle rare cases where
   // "lost" data arrives after gap is forced
   var entry = await logger.ReadAsync(1);
   if (await WasAlreadyProcessed(entry.Data.Id))
   {
       await logger.CommitAsync(entry.GetPositions()); // Just commit, don't reprocess
       return;
   }
   await ProcessData(entry.Data);
   await MarkAsProcessed(entry.Data.Id);
   await logger.CommitAsync(entry.GetPositions());
   ```

#### Gap Lifecycle

```
1. Gap Detected
   ↓
2. Warning Logged (after GapTimeoutMillis, default 10 min)
   ↓
3a. Data Arrives → Gap Filled → Progress Continues
   OR
3b. Automatic Force (after ForceCompleteGapTimeoutMillis, if enabled)
   OR
3c. Manual Force (via ForceCommitGap API)
   ↓
4. Gap Skipped → Progress Resumes → ⚠️ Data Lost
```

### Memory Management

The module implements multiple memory protection mechanisms:

1. **Bounded Channel**: Limits buffered entries via `PreReadCapacity`
2. **Range Limit**: Caps completed ranges at `MaxCompletedRanges`
3. **Automatic Cleanup**: Removes oldest ranges when limit exceeded
4. **Persistence Optimization**: Only persists ranges beyond truncate address
5. **Graceful Shutdown**: Waits for background tasks with 5-second timeout

### Thread Safety

All operations are thread-safe:
- Uses `Interlocked` operations for atomic counters
- `SemaphoreSlim` for initialization and commit synchronization
- `lock` for completed ranges collection access
- Cancellation token support for graceful shutdown

## Best Practices

### 1. Configuration Tuning

```csharp
// For high-throughput scenarios
"CommitIntervalMillis": 5000,      // Less frequent commits
"PreReadCapacity": 10000,          // Larger buffer
"MaxCompletedRanges": 50000        // More gap tracking

// For low-latency scenarios
"CommitIntervalMillis": 100,       // Frequent commits
"PreReadCapacity": 1000,           // Smaller buffer
"GapTimeoutMillis": 30000          // Quick gap detection
```

### 2. Error Handling

```csharp
try
{
    await _logger.WriteAsync(data);
}
catch (InvalidOperationException ex)
{
    // Logger not initialized or configuration error
    _logger.Initialize();
}
catch (OperationCanceledException)
{
    // Shutdown in progress
}
```

### 3. Resource Cleanup

The module implements `IDisposable` properly:

```csharp
// Factory will be disposed by ABP DI container
// Individual loggers are disposed when factory disposes
// Manual disposal is not required
```

### 4. Monitoring and Alerting

```csharp
// Set up periodic monitoring
if (_logger.CurrentGapCount > 100)
{
    _logger.LogWarning($"High gap count: {_logger.CurrentGapCount}");
}

if (_logger.LargestGapSize > 1_000_000)
{
    _logger.LogWarning($"Large gap detected: {_logger.LargestGapSize} bytes");
}
```

## Troubleshooting

### Issue: Gaps Not Clearing

**Symptoms**: `CurrentGapCount` stays high, `GapTimeoutMillis` warnings

**Possible Causes**:
1. Some entries never get committed
2. Processing threads are stuck or crashed
3. Commit logic has bugs

**Solutions**:
- Check if all read entries are being committed
- Review processing thread health
- Verify commit logic correctness

### Issue: High Memory Usage

**Symptoms**: Memory grows unbounded

**Possible Causes**:
1. `MaxCompletedRanges` set too high
2. `PreReadCapacity` too large
3. Many gaps preventing range merging

**Solutions**:
- Reduce `MaxCompletedRanges`
- Decrease `PreReadCapacity`
- Investigate why gaps are forming

### Issue: Data Loss

**Symptoms**: Written data not appearing in reads

**Possible Causes**:
1. Entries written but not committed
2. Application crashed before commit interval
3. Truncate happened before read

**Solutions**:
- Always commit after writing
- Reduce `CommitIntervalMillis` for critical data
- Ensure proper shutdown handling

## Version History

### Recent Improvements

#### v2.0 - Gap-Based Architecture
- ✅ Replaced retry-based commit with interval tracking
- ✅ Added gap timeout detection and warnings
- ✅ Implemented memory protection limits
- ✅ Added comprehensive monitoring metrics

#### v2.1 - Resource Management
- ✅ Implemented `IDisposable` for proper cleanup
- ✅ Added graceful background task shutdown
- ✅ Fixed channel completion on dispose
- ✅ Enhanced exception handling

#### v2.2 - Validation & Safety
- ✅ Added configuration validation
- ✅ Parameter validation for all constructors
- ✅ Protection against multiple dispose calls
- ✅ Improved async/cancellation handling

#### v2.3 - Range Persistence
- ✅ **Implemented range persistence** to prevent data reprocessing after restart
- ✅ Automatic persistence every 5 seconds (configurable)
- ✅ Final persistence on graceful shutdown
- ✅ Optimized storage (only persists ranges beyond truncate)
- ✅ Independent persistence files per logger configuration
- ✅ Fixed memory protection bug (was deleting newest instead of oldest ranges)
- ✅ Fixed progress loss on CompleteUntilRecordAtAsync failure
- ✅ Removed Position parameterless constructor (prevents invalid objects)
- ✅ Changed Position properties to prevent modification after creation

#### v2.4 - Force Complete Mechanism (Current)
- ✅ **Implemented force complete mechanism** to handle persistent gaps
- ✅ Automatic gap skipping after configurable timeout (`ForceCompleteGapTimeoutMillis`)
- ✅ Manual gap filling API (`ForceCommitGap`) for emergency intervention
- ✅ Comprehensive documentation with warnings about data loss risks
- ✅ Gap lifecycle tracking and enhanced logging
- ✅ Prevents indefinite blocking when commit data is permanently lost

## License

This module is part of the SharpAbp framework.

## Contributing

Contributions are welcome! Please ensure:
- Thread safety in concurrent scenarios
- Proper resource disposal
- Comprehensive error handling
- Unit tests for new features
