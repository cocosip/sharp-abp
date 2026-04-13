using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.Faster
{
    public class FasterLoggerTests : AbpFasterTestBase
    {
        // Delay longer than CommitIntervalMillis(100ms) + CompleteIntervalMillis(200ms)
        private const int CommitWaitMs = 400;

        private readonly IFasterLoggerFactory _factory;
        private readonly IFasterLogger<FasterTestEntry> _logger;
        private readonly string _logName;

        public FasterLoggerTests()
        {
            _factory = GetRequiredService<IFasterLoggerFactory>();
            _logName = FasterLogNameAttribute.GetLogName<FasterTestEntry>();
            _logger = _factory.GetOrCreate<FasterTestEntry>(_logName);
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
            var beforeAddress = _logger.CommittedUntilAddress;

            await _logger.WriteAsync(new FasterTestEntry { Id = 100, Value = "commit-test" });

            // Wait for background commit task
            await Task.Delay(CommitWaitMs);

            Assert.True(_logger.CommittedUntilAddress > beforeAddress);
        }

        [Fact]
        public async Task ReadAsync_ShouldReturnWrittenEntry()
        {
            await _logger.WriteAsync(new FasterTestEntry { Id = 200, Value = "read-test" });

            // Wait for commit + scan to push into channel
            await Task.Delay(CommitWaitMs);

            var entries = await _logger.ReadAsync(count: 1);

            Assert.NotEmpty(entries);
            Assert.NotNull(entries[0].Data);
            Assert.True(entries[0].CurrentAddress >= 0);
            Assert.True(entries[0].NextAddress > entries[0].CurrentAddress);
        }

        [Fact]
        public async Task CommitAsync_ShouldTrackPositions()
        {
            await _logger.WriteAsync(new FasterTestEntry { Id = 300, Value = "commit-pos-test" });
            await Task.Delay(CommitWaitMs);

            var entries = await _logger.ReadAsync(count: 1);
            Assert.NotEmpty(entries);

            await _logger.CommitAsync(entries.GetPositions());

            Assert.True(_logger.TotalCommittedRanges > 0);
        }

        [Fact]
        public async Task ExportAsync_ShouldCreateFile_WhenDataExists()
        {
            await _logger.WriteAsync(new FasterTestEntry { Id = 400, Value = "export-test" });
            await Task.Delay(CommitWaitMs);

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
            // Write 5 entries
            for (var i = 0; i < 5; i++)
            {
                await _logger.WriteAsync(new FasterTestEntry { Id = 500 + i, Value = $"split-{i}" });
            }
            await Task.Delay(CommitWaitMs);

            var exportDir = Path.Combine(Path.GetTempPath(), $"faster-export-{Guid.NewGuid():N}");
            try
            {
                var result = await _logger.ExportAsync(exportDir, _logger.BeginAddress, entriesPerFile: 2);

                Assert.True(result.Count >= 5);
                // 5 entries with 2 per file = at least 3 files
                Assert.True(result.FilePaths.Count >= 3);
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
            await Task.Delay(CommitWaitMs);

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
            await _logger.WriteAsync(new FasterTestEntry { Id = 600, Value = "json-check" });
            await Task.Delay(CommitWaitMs);

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
            await _logger.WriteAsync(new FasterTestEntry { Id = 700, Value = "incremental-1" });
            await Task.Delay(CommitWaitMs);

            var exportDir = Path.Combine(Path.GetTempPath(), $"faster-export-{Guid.NewGuid():N}");
            try
            {
                // First export
                var result1 = await _logger.ExportAsync(exportDir, _logger.BeginAddress);

                // Write more data
                await _logger.WriteAsync(new FasterTestEntry { Id = 701, Value = "incremental-2" });
                await Task.Delay(CommitWaitMs);

                // Second incremental export starting from where we left off
                var result2 = await _logger.ExportAsync(exportDir, result1.ToAddress);

                Assert.True(result2.Count >= 1);
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
            await Task.Delay(CommitWaitMs);

            var readers = Enumerable.Range(0, 3)
                .Select(_ => Task.Run(async () =>
                {
                    var batch = await concurrentLogger.ReadAsync(4);
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
            await Task.Delay(CommitWaitMs);

            var entries = await lateCommitLogger.ReadAsync(2);
            Assert.Equal(2, entries.Count);

            var first = entries[0];
            var second = entries[1];

            await lateCommitLogger.CommitAsync(new[] { new Position(second.CurrentAddress, second.NextAddress) });
            await lateCommitLogger.ForceCommitGap(first.CurrentAddress, first.NextAddress);
            await Task.Delay(CommitWaitMs * 2);

            Assert.True(lateCommitLogger.TruncateBeforeAddress >= second.NextAddress);

            var committedBeforeLateCommit = lateCommitLogger.TotalCommittedRanges;
            var rangesBeforeLateCommit = lateCommitLogger.CompletedRangeCount;

            await lateCommitLogger.CommitAsync(new[] { new Position(first.CurrentAddress, first.NextAddress) });

            Assert.Equal(committedBeforeLateCommit, lateCommitLogger.TotalCommittedRanges);
            Assert.Equal(rangesBeforeLateCommit, lateCommitLogger.CompletedRangeCount);
        }
    }
}
