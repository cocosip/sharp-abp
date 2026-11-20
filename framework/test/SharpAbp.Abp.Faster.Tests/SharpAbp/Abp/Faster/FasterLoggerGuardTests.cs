using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using FASTER.core;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Volo.Abp.Serialization;
using Volo.Abp.Threading;
using Xunit;

namespace SharpAbp.Abp.Faster;

public class FasterLoggerGuardTests : AbpFasterTestBase
{
    [Fact]
    public async Task Uninitialized_Logger_Should_Reject_Writes_Export_And_Forced_Completion()
    {
        using var logger = CreateLogger(Path.GetTempPath());
        await Assert.ThrowsAsync<InvalidOperationException>(() => logger.WriteAsync(new FasterTestEntry()));
        await Assert.ThrowsAsync<InvalidOperationException>(() => logger.BatchWriteAsync([new FasterTestEntry()]));
        await Assert.ThrowsAsync<InvalidOperationException>(() => logger.ExportAsync(Path.GetTempPath(), 0));
        await Assert.ThrowsAsync<InvalidOperationException>(() => logger.ForceCommitGap(0, 1));
    }

    [Fact]
    public async Task Writes_And_Commits_Should_Reject_Null_Payloads()
    {
        using var logger = CreateLogger(Path.GetTempPath());
        await Assert.ThrowsAsync<ArgumentNullException>(() => logger.WriteAsync(null));
        await Assert.ThrowsAsync<ArgumentNullException>(() => logger.BatchWriteAsync(null));
        await Assert.ThrowsAsync<ArgumentNullException>(() => logger.CommitAsync(null));
    }

    [Fact]
    public void Missing_Root_Path_Should_Fail_Initialization_And_Leave_Logger_Uninitialized()
    {
        using var logger = CreateLogger(" ");
        Assert.Throws<InvalidOperationException>(() => logger.Initialize());
        Assert.False(logger.Initialized);
    }

    [Fact]
    public async Task Write_And_Read_Counters_Should_Track_Real_Entries_Without_Reinitializing_Log()
    {
        var directory = Directory.CreateTempSubdirectory("sharp-abp-faster-guards-");
        try
        {
            using var logger = CreateLogger(directory.FullName);
            logger.Initialize();
            await logger.WriteAsync(new FasterTestEntry { Id = 1, Value = "single" });
            logger.Initialize();
            await logger.BatchWriteAsync([new FasterTestEntry { Id = 2, Value = "batch-one" }, new FasterTestEntry { Id = 3, Value = "batch-two" }]);
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var first = Assert.Single(await logger.ReadAsync(cancellationToken: timeout.Token));
            var second = Assert.Single(await logger.ReadAsync(cancellationToken: timeout.Token));
            var third = Assert.Single(await logger.ReadAsync(cancellationToken: timeout.Token));

            Assert.Equal(new[] { 1, 2, 3 }, new[] { first.Data.Id, second.Data.Id, third.Data.Id });
            Assert.Equal(3, logger.TotalWriteCount);
            Assert.Equal(3, logger.TotalReadCount);
        }
        finally
        {
            directory.Delete(true);
        }
    }

    [Theory]
    [InlineData(-1, 1)]
    [InlineData(2, 2)]
    [InlineData(3, 2)]
    public async Task Forced_Gap_Completion_Should_Reject_Invalid_Ranges(long start, long end)
    {
        var directory = Directory.CreateTempSubdirectory("sharp-abp-faster-guards-");
        try
        {
            using var logger = CreateLogger(directory.FullName);
            logger.Initialize();
            await Assert.ThrowsAsync<ArgumentException>(() => logger.ForceCommitGap(start, end));
            Assert.Equal(0, logger.CompletedRangeCount);
        }
        finally
        {
            directory.Delete(true);
        }
    }

    [Fact]
    public async Task Commit_Should_Ignore_Stale_Overlapping_And_Duplicate_Ranges()
    {
        var directory = Directory.CreateTempSubdirectory("sharp-abp-faster-guards-");
        try
        {
            using var logger = CreateLogger(directory.FullName);
            logger.Initialize();
            var boundary = logger.TruncateBeforeAddress;
            Assert.True(boundary > 0);
            var valid = new Position(boundary + 5, boundary + 10);

            await logger.CommitAsync(new[] { new Position(0, boundary), new Position(boundary - 1, boundary + 1), valid, valid }.Select(position => position));

            Assert.Equal(1, logger.TotalCommittedRanges);
            Assert.Equal(1, logger.CompletedRangeCount);
            await logger.ForceCommitGap(boundary + 20, boundary + 30);
            await logger.ForceCommitGap(boundary + 20, boundary + 30);
            Assert.Equal(2, logger.CompletedRangeCount);
            Assert.Equal(1, logger.TotalCommittedRanges);
        }
        finally
        {
            directory.Delete(true);
        }
    }

    [Fact]
    public async Task Export_Before_Begin_Should_Adjust_To_Existing_Log_Without_Creating_Empty_File()
    {
        var directory = Directory.CreateTempSubdirectory("sharp-abp-faster-guards-");
        try
        {
            using var logger = CreateLogger(directory.FullName);
            logger.Initialize();
            var target = Path.Combine(directory.FullName, "export");

            var result = await logger.ExportAsync(target, -1, logger.BeginAddress);

            Assert.Equal(logger.BeginAddress, result.FromAddress);
            Assert.Equal(logger.BeginAddress, result.ToAddress);
            Assert.Equal(0, result.Count);
            Assert.Empty(result.FilePaths);
            Assert.False(Directory.Exists(target));
        }
        finally
        {
            directory.Delete(true);
        }
    }

    [Fact]
    public async Task Disposing_Should_Release_Blocked_Reader_And_Allow_Repeated_Dispose()
    {
        using var logger = CreateLogger(Path.GetTempPath());
        var pending = logger.ReadAsync();

        logger.Dispose();
        logger.Dispose();

        await Assert.ThrowsAsync<ChannelClosedException>(() => pending);
    }

    [Fact]
    public async Task Disposing_Initialized_Logger_Should_Release_Owned_Log_Device()
    {
        var directory = Directory.CreateTempSubdirectory("sharp-abp-faster-dispose-");
        IDevice device = null;
        var released = false;
        try
        {
            using var logger = CreateLogger(directory.FullName, settings => device = settings.LogDevice);
            logger.Initialize();
            await logger.WriteAsync(new FasterTestEntry { Id = 1, Value = "dispose" });
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            Assert.Single(await logger.ReadAsync(cancellationToken: timeout.Token));

            logger.Dispose();

            var file = Assert.Single(directory.GetFiles("guards.log.0", SearchOption.AllDirectories));
            using var exclusive = File.Open(file.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            released = true;
        }
        finally
        {
            if (!released)
            {
                device?.Dispose();
            }
            directory.Delete(true);
        }
    }

    private FasterLogger<FasterTestEntry> CreateLogger(string rootPath, Action<FasterLogSettings> configure = null) => new(
        GetRequiredService<ILogger<FasterLogger<FasterTestEntry>>>(),
        Options.Create(new AbpFasterOptions { RootPath = rootPath }),
        "guards",
        new AbpFasterConfiguration
        {
            FileName = "guards.log",
            PreallocateFile = false,
            PageSizeBits = 12,
            MemorySizeBits = 14,
            SegmentSizeBits = 20,
            Configure = configure,
            CommitIntervalMillis = 20,
            CompleteIntervalMillis = 600000,
            TruncateIntervalMillis = 600000
        },
        GetRequiredService<IObjectSerializer>(),
        GetRequiredService<ICancellationTokenProvider>());
}
