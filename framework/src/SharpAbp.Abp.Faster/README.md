# SharpAbp.Abp.Faster

## Configuration

This module uses the following configuration structure in `appsettings.json`:

```json
{
  "FasterOptions": {
    "RootPath": "/data/faster-logs",
    "Configurations": {
      "default1": {
        "FileName": "log1",
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
        "MaxCommitSkip": 10,
        "IteratorName": "l1"
      },
      "default2": {
        "FileName": "log2",
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
        "MaxCommitSkip": 10,
        "IteratorName": "l2"
      }
    }
  }
}
```

## Configuration Options

### FasterOptions
- **RootPath**: The root directory path for storing faster log files
- **Configurations**: Dictionary of named configurations for different log instances

### Configuration Properties
- **FileName**: Name of the log file
- **PreallocateFile**: Whether to preallocate the log file
- **Capacity**: Maximum capacity of the log file in bytes
- **RecoverDevice**: Whether to recover the device on startup
- **UseIoCompletionPort**: Whether to use I/O completion ports
- **DisableFileBuffering**: Whether to disable file buffering
- **ScanUncommitted**: Whether to scan uncommitted entries
- **AutoRefreshSafeTailAddress**: Whether to automatically refresh safe tail address
- **CommitIntervalMillis**: Commit interval in milliseconds
- **CompleteIntervalMillis**: Complete interval in milliseconds
- **TruncateIntervalMillis**: Truncate interval in milliseconds
- **PreReadCapacity**: Pre-read capacity for performance optimization
- **MaxCommitSkip**: Maximum number of commits to skip
- **IteratorName**: Name identifier for the iterator