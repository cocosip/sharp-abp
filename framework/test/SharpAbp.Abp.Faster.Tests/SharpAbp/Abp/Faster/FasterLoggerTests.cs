using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Xunit;

namespace SharpAbp.Abp.Faster
{
    public class FasterLoggerTests : AbpFasterTestBase
    {
        private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(10);

        private readonly DirectoryInfo _logDirectory = Directory.CreateTempSubdirectory("sharp-abp-faster-");

        private readonly IFasterLoggerFactory _factory;
        private readonly IFasterLogger<FasterTestEntry> _logger;
        private readonly string _logName;

        public FasterLoggerTests()
        {
            GetRequiredService<IOptions<AbpFasterOptions>>().Value.RootPath = _logDirectory.FullName;
            _factory = GetRequiredService<IFasterLoggerFactory>();
            _logName = FasterLogNameAttribute.GetLogName<FasterTestEntry>();
            _logger = _factory.GetOrCreate<FasterTestEntry>(_logName);
        }

        public override void Dispose()
        {
            try
            {
                base.Dispose();
            }
            finally
            {
                _logDirectory.Delete(true);
            }
        }

        [Fact]
        public void Initialized_ShouldBeTrue()
        {
            Assert.True(_logger.Initialized);
        }

        [Fact]
        public void BeginAddress_ShouldBeNonNegative()
        {
            Assert.True(_logger.BeginAddress >= 0);
        }

        [Fact]
        public void IFasterLogger_ShouldNotResolveDirectlyFromDependencyInjection()
        {
            Assert.Null(GetService<IFasterLogger>());
            Assert.Null(GetService<IFasterLogger<FasterTestEntry>>());
        }

        [Fact]
        public async Task WriteAsync_ShouldReturnPositiveAddress()
        {
            var address = await _logger.WriteAsync(new FasterTestEntry { Id = 1, Value = "hello" });
            Assert.True(address >= 0);
        }

        [Fact]
        public async Task BatchWriteAsync_ShouldReturnCorrectCount()
        {
            var items = new List<FasterTestEntry>
            {
                new FasterTestEntry { Id = 10, Value = "a" },
                new FasterTestEntry { Id = 11, Value = "b" },
                new FasterTestEntry { Id = 12, Value = "c" },
            };

            var addresses = await _logger.BatchWriteAsync(items);

            Assert.Equal(3, addresses.Count);
            Assert.All(addresses, addr => Assert.True(addr >= 0));
        }

        [Fact]
        public async Task WriteAsync_ThenCommit_ShouldAdvanceCommittedUntilAddress()
        {
            var address = await _logger.WriteAsync(new FasterTestEntry { Id = 100, Value = "commit-test" });
            await WaitForCommitAsync(_logger, address);
        }

        [Fact]
        public async Task ReadAsync_ShouldReturnWrittenEntry()
        {
            await _logger.WriteAsync(new FasterTestEntry { Id = 200, Value = "read-test" });

            var entries = await ReadEntriesAsync(_logger, 1);
            Assert.Equal(200, entries[0].Data!.Id);
            Assert.True(entries[0].CurrentAddress >= 0);
            Assert.True(entries[0].NextAddress > entries[0].CurrentAddress);
        }

        [Fact]
        public async Task CommitAsync_ShouldTrackPositions()
        {
            await _logger.WriteAsync(new FasterTestEntry { Id = 300, Value = "commit-pos-test" });
            var entries = await ReadEntriesAsync(_logger, 1);

            await _logger.CommitAsync(entries.GetPositions());

            Assert.True(_logger.TotalCommittedRanges > 0);
        }

        [Fact]
        public async Task ExportAsync_ShouldCreateFile_WhenDataExists()
        {
            var address = await _logger.WriteAsync(new FasterTestEntry { Id = 400, Value = "export-test" });
            await WaitForCommitAsync(_logger, address);

            var exportDir = Path.Combine(Path.GetTempPath(), $"faster-export-{Guid.NewGuid():N}");
            try
            {
                var result = await _logger.ExportAsync(exportDir, _logger.BeginAddress);

                Assert.True(result.Count > 0);
                Assert.NotEmpty(result.FilePaths);
                Assert.True(result.ToAddress > result.FromAddress);
                Assert.All(result.FilePaths, path => Assert.True(File.Exists(path)));
            }
            finally
            {
                if (Directory.Exists(exportDir))
                    Directory.Delete(exportDir, true);
            }
        }

        [Fact]
        public async Task ExportAsync_ShouldSplitFiles_WhenEntriesExceedPerFileLimit()
        {
            var addresses = await _logger.BatchWriteAsync(Enumerable.Range(0, 5)
                .Select(i => new FasterTestEntry { Id = 500 + i, Value = $"split-{i}" }).ToList());
            await WaitForCommitAsync(_logger, addresses[^1]);

            var exportDir = Path.Combine(Path.GetTempPath(), $"faster-export-{Guid.NewGuid():N}");
            try
            {
                var result = await _logger.ExportAsync(exportDir, _logger.BeginAddress, entriesPerFile: 2);

                Assert.Equal(5, result.Count);
                Assert.Equal(new[] { 2, 2, 1 }, result.FilePaths.Select(path => File.ReadLines(path).Count()));
            }
            finally
            {
                if (Directory.Exists(exportDir))
                    Directory.Delete(exportDir, true);
            }
        }

        [Fact]
        public async Task ExportAsync_NoNewData_ShouldReturnZeroCount()
        {
            var committedAddress = _logger.CommittedUntilAddress;
            var exportDir = Path.Combine(Path.GetTempPath(), $"faster-export-{Guid.NewGuid():N}");
            try
            {
                // Export from committed address to committed address = nothing to export
                var result = await _logger.ExportAsync(exportDir, committedAddress, committedAddress);

                Assert.Equal(0, result.Count);
                Assert.Empty(result.FilePaths);
            }
            finally
            {
                if (Directory.Exists(exportDir))
                    Directory.Delete(exportDir, true);
            }
        }

        [Fact]
        public async Task ExportAsync_FilesShouldContainJsonLines()
        {
            var address = await _logger.WriteAsync(new FasterTestEntry { Id = 600, Value = "json-check" });
            await WaitForCommitAsync(_logger, address);

            var exportDir = Path.Combine(Path.GetTempPath(), $"faster-export-{Guid.NewGuid():N}");
            try
            {
                var result = await _logger.ExportAsync(exportDir, _logger.BeginAddress);

                Assert.NotEmpty(result.FilePaths);

                // Each line in the .jsonl file should be non-empty
                var lines = await File.ReadAllLinesAsync(result.FilePaths[0]);
                Assert.NotEmpty(lines);
                Assert.All(lines, line => Assert.False(string.IsNullOrWhiteSpace(line)));
            }
            finally
            {
                if (Directory.Exists(exportDir))
                    Directory.Delete(exportDir, true);
            }
        }

        [Fact]
        public async Task ExportAsync_IncrementalExport_ShouldOnlyExportNewData()
        {
            var firstAddress = await _logger.WriteAsync(new FasterTestEntry { Id = 700, Value = "incremental-1" });
            await WaitForCommitAsync(_logger, firstAddress);

            var exportDir = Path.Combine(Path.GetTempPath(), $"faster-export-{Guid.NewGuid():N}");
            try
            {
                // First export
                var result1 = await _logger.ExportAsync(exportDir, _logger.BeginAddress);
                Assert.Equal(1, result1.Count);

                // Write more data
                var secondAddress = await _logger.WriteAsync(new FasterTestEntry { Id = 701, Value = "incremental-2" });
                await WaitForCommitAsync(_logger, secondAddress);

                // Second incremental export starting from where we left off
                var result2 = await _logger.ExportAsync(exportDir, result1.ToAddress);

                Assert.Equal(1, result2.Count);
                // Incremental export should not overlap with the first export
                Assert.True(result2.FromAddress >= result1.ToAddress);
            }
            finally
            {
                if (Directory.Exists(exportDir))
                    Directory.Delete(exportDir, true);
            }
        }

        [Fact]
        public void GetOrCreate_WithSameNameAndDifferentTypes_ShouldReturnDistinctInstances()
        {
            var loggerA = _factory.GetOrCreate<FasterAlternateEntryA>("shared-cache-key");
            var loggerB = _factory.GetOrCreate<FasterAlternateEntryB>("shared-cache-key");

            Assert.NotSame((object)loggerA, loggerB);
            Assert.True(loggerA.Initialized);
            Assert.True(loggerB.Initialized);
        }

        [Fact]
        public async Task ReadAsync_ShouldSupportConcurrentConsumers()
        {
            var concurrentLogger = _factory.GetOrCreate<FasterConcurrentTestEntry>(
                FasterLogNameAttribute.GetLogName<FasterConcurrentTestEntry>());
            var ids = Enumerable.Range(800, 12)
                .Select(id => new FasterConcurrentTestEntry { Id = id })
                .ToList();

            await concurrentLogger.BatchWriteAsync(ids);

            var readers = Enumerable.Range(0, 3)
                .Select(_ => Task.Run(async () =>
                {
                    var batch = await ReadEntriesAsync(concurrentLogger, 4);
                    await concurrentLogger.CommitAsync(batch.GetPositions());
                    return batch.Select(x => x.Data!.Id).ToList();
                }))
                .ToArray();

            var results = await Task.WhenAll(readers);
            var allIds = results.SelectMany(x => x).OrderBy(x => x).ToList();

            Assert.Equal(ids.Count, allIds.Count);
            Assert.Equal(ids.Select(x => x.Id).OrderBy(x => x), allIds);
        }

        [Fact]
        public async Task CommitAsync_ShouldIgnoreLateCommits_AfterGapWasForceCompleted()
        {
            var lateCommitLogger = _factory.GetOrCreate<FasterLateCommitTestEntry>(
                FasterLogNameAttribute.GetLogName<FasterLateCommitTestEntry>());

            await lateCommitLogger.BatchWriteAsync(new List<FasterLateCommitTestEntry>
            {
                new FasterLateCommitTestEntry { Id = 1 },
                new FasterLateCommitTestEntry { Id = 2 }
            });
            var entries = await ReadEntriesAsync(lateCommitLogger, 2);

            var first = entries[0];
            var second = entries[1];

            await lateCommitLogger.CommitAsync(new[] { new Position(second.CurrentAddress, second.NextAddress) });
            await lateCommitLogger.ForceCommitGap(first.CurrentAddress, first.NextAddress);
            await WaitForAddressAsync(() => lateCommitLogger.TruncateBeforeAddress, second.NextAddress);

            var committedBeforeLateCommit = lateCommitLogger.TotalCommittedRanges;
            var rangesBeforeLateCommit = lateCommitLogger.CompletedRangeCount;

            await lateCommitLogger.CommitAsync(new[] { new Position(first.CurrentAddress, first.NextAddress) });

            Assert.Equal(committedBeforeLateCommit, lateCommitLogger.TotalCommittedRanges);
            Assert.Equal(rangesBeforeLateCommit, lateCommitLogger.CompletedRangeCount);
        }

        [Fact]
        public async Task GetOrCreate_ShouldInitializeGenericTypeLoggers_WithFilesystemSafePaths()
        {
            var logger = _factory.GetOrCreate<FasterGenericPayloadEntry<string>>("faster-generic-payload-entry");

            Assert.True(logger.Initialized);

            var address = await logger.WriteAsync(new FasterGenericPayloadEntry<string> { Payload = "hello" });

            Assert.True(address >= 0);
        }

        [Fact]
        public async Task GetOrCreate_ShouldRecoverAfterInitializationFailure()
        {
            const string logName = "recoverable-init-entry";
            var options = GetRequiredService<IOptions<AbpFasterOptions>>().Value;

            var exception = Assert.Throws<InvalidOperationException>(
                () => _factory.GetOrCreate<FasterRecoverableInitEntry>(logName));

            Assert.Contains("FileName must be configured", exception.Message);

            options.Configurations.Configure(logName, c =>
            {
                c.FileName = "recoverable-init-entry.log";
                c.PreallocateFile = false;
                c.CommitIntervalMillis = 100;
                c.CompleteIntervalMillis = 200;
                c.TruncateIntervalMillis = 600000;
            });

            var recoveredLogger = _factory.GetOrCreate<FasterRecoverableInitEntry>(logName);
            var address = await recoveredLogger.WriteAsync(new FasterRecoverableInitEntry { Id = 1 });

            Assert.True(recoveredLogger.Initialized);
            Assert.True(address >= 0);
        }

        [Fact]
        public async Task CommitAsync_ShouldForceOldestGap_WhenCompletedRangeLimitIsExceeded()
        {
            var rangeLimitLogger = _factory.GetOrCreate<FasterRangeLimitTestEntry>(
                FasterLogNameAttribute.GetLogName<FasterRangeLimitTestEntry>());

            await rangeLimitLogger.BatchWriteAsync(new List<FasterRangeLimitTestEntry>
            {
                new FasterRangeLimitTestEntry { Id = 1 },
                new FasterRangeLimitTestEntry { Id = 2 },
                new FasterRangeLimitTestEntry { Id = 3 },
                new FasterRangeLimitTestEntry { Id = 4 }
            });
            var entries = await ReadEntriesAsync(rangeLimitLogger, 4);

            await rangeLimitLogger.CommitAsync(entries.Skip(1).Select(x => new Position(x.CurrentAddress, x.NextAddress)));
            await WaitForAddressAsync(() => rangeLimitLogger.TruncateBeforeAddress, entries[^1].NextAddress);

            var committedBeforeLateCommit = rangeLimitLogger.TotalCommittedRanges;
            await rangeLimitLogger.CommitAsync(new[] { new Position(entries[0].CurrentAddress, entries[0].NextAddress) });

            Assert.Equal(committedBeforeLateCommit, rangeLimitLogger.TotalCommittedRanges);
        }

        private static Task WaitForCommitAsync<T>(IFasterLogger<T> logger, long entryAddress) where T : class =>
            WaitForAddressAsync(() => logger.CommittedUntilAddress, entryAddress + 1);

        private static async Task WaitForAddressAsync(Func<long> getAddress, long minimumAddress)
        {
            var timer = Stopwatch.StartNew();
            while (getAddress() < minimumAddress && timer.Elapsed < TestTimeout)
            {
                await Task.Delay(20);
            }

            var address = getAddress();
            Assert.True(address >= minimumAddress, $"Log address reached {address}, expected at least {minimumAddress}.");
        }

        private static async Task<LogEntryList<T>> ReadEntriesAsync<T>(IFasterLogger<T> logger, int count) where T : class
        {
            using var timeout = new CancellationTokenSource(TestTimeout);
            var entries = new LogEntryList<T>();
            while (entries.Count < count)
            {
                entries.AddRange(await logger.ReadAsync(count - entries.Count, timeout.Token));
            }

            return entries;
        }
    }
}
