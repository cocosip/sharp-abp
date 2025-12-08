# SharpAbp.Abp.Faster

A high-performance logging module built on [Microsoft FASTER](https://github.com/microsoft/FASTER) for ABP framework, providing efficient append-only log storage with support for multi-threaded writes and out-of-order commits.

## Features

- ✅ **High-Performance Logging**: Built on Microsoft FASTER for efficient log operations
- ✅ **Multi-Threaded Support**: Safe concurrent writes and reads
- ✅ **Out-of-Order Commit**: Handles non-sequential commit scenarios using gap-based interval tracking
- ✅ **Monitoring Metrics**: Built-in observability with performance counters
- ✅ **Resource Management**: Automatic cleanup and graceful shutdown
- ✅ **Memory Protection**: Configurable limits to prevent unbounded memory growth
- ✅ **Gap Detection**: Timeout warnings for stuck processing

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
        "MaxCompletedRanges": 10000,
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
| `MaxCompletedRanges` | `int` | `10000` | Maximum number of ranges to track (memory protection) |
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

### Memory Management

The module implements multiple memory protection mechanisms:

1. **Bounded Channel**: Limits buffered entries via `PreReadCapacity`
2. **Range Limit**: Caps completed ranges at `MaxCompletedRanges`
3. **Automatic Cleanup**: Removes excess ranges when limit is exceeded
4. **Graceful Shutdown**: Waits for background tasks with 5-second timeout

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

## License

This module is part of the SharpAbp framework.

## Contributing

Contributions are welcome! Please ensure:
- Thread safety in concurrent scenarios
- Proper resource disposal
- Comprehensive error handling
- Unit tests for new features
