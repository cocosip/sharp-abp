#nullable enable
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using SharpAbp.Abp.FileStoring.FileSystem;
using Volo.Abp.MultiTenancy;
using Xunit;

namespace SharpAbp.Abp.FileStoring
{
    public class FileSystemAtomicSaveTests : IDisposable
    {
        private readonly string _basePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        private readonly FileContainerConfiguration _configuration = new();
        private readonly DefaultFilePathCalculator _calculator;
        private readonly FileSystemFileProvider _provider;

        public FileSystemAtomicSaveTests()
        {
            _configuration.HttpAccess = true;
            _configuration.UseFileSystem(options =>
            {
                options.BasePath = _basePath;
                options.HttpServer = "https://cdn.example.com/files";
            });
            var tenant = new Mock<ICurrentTenant>();
            _calculator = new DefaultFilePathCalculator(tenant.Object, Mock.Of<IFilePathContextAccessor>(),
                Options.Create(new AbpFileStoringAbstractionsOptions()));
            _provider = new FileSystemFileProvider(NullLogger<FileSystemFileProvider>.Instance, tenant.Object, _calculator);
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        }

        private FileProviderSaveArgs Args(Stream stream, bool overwrite = false, CancellationToken cancellationToken = default)
            => new("docs", _configuration, "file.txt", stream, ".txt", overwrite, cancellationToken);

        private string FilePath => _calculator.Calculate(new FileProviderSaveArgs("docs", _configuration, "file.txt"));

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(true, true)]
        public async Task Failed_Or_Cancelled_Copy_Should_Not_Publish_Partial_Content(bool overwrite, bool cancel)
        {
            if (overwrite)
            {
                await File.WriteAllTextAsync(FilePath, "original");
            }
            using var cancellation = new CancellationTokenSource();
            using var stream = new ControlledCopyStream(async () =>
            {
                if (cancel)
                {
                    cancellation.Cancel();
                    cancellation.Token.ThrowIfCancellationRequested();
                }
                await Task.CompletedTask;
                throw new IOException("Read failed after copying a prefix");
            });

            var exception = await Record.ExceptionAsync(() => _provider.SaveAsync(Args(stream, overwrite, cancellation.Token)));

            if (cancel)
            {
                Assert.IsAssignableFrom<OperationCanceledException>(exception);
            }
            else
            {
                Assert.IsType<IOException>(exception);
            }
            Assert.Equal(1, stream.CopyCount); // A consumed non-seekable stream cannot be retried.
            if (overwrite)
            {
                Assert.Equal("original", await File.ReadAllTextAsync(FilePath));
            }
            else
            {
                Assert.False(File.Exists(FilePath));
            }
            Assert.Equal(overwrite ? new[] { FilePath } : Array.Empty<string>(), Directory.GetFiles(_basePath, "*", SearchOption.AllDirectories));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task Exhausted_Seekable_Retries_Should_Preserve_Original_And_Clean_Staging(bool overwrite)
        {
            if (overwrite)
            {
                await File.WriteAllTextAsync(FilePath, "original");
            }
            using var stream = new ControlledCopyStream(() => throw new IOException("Persistent read failure"), canSeek: true);

            await Assert.ThrowsAsync<IOException>(() => _provider.SaveAsync(Args(stream, overwrite)));

            Assert.Equal(3, stream.CopyCount);
            if (overwrite)
            {
                Assert.Equal("original", await File.ReadAllTextAsync(FilePath));
            }
            else
            {
                Assert.False(File.Exists(FilePath));
            }
            Assert.Equal(overwrite ? new[] { FilePath } : Array.Empty<string>(), Directory.GetFiles(_basePath, "*", SearchOption.AllDirectories));
        }

        [Fact]
        public async Task Overwrite_Should_Preserve_Content_And_Supported_Creation_Metadata()
        {
            await File.WriteAllTextAsync(FilePath, "original");
            var created = new DateTime(2001, 2, 3, 4, 5, 6, DateTimeKind.Utc);
            if (OperatingSystem.IsWindows())
            {
                File.SetCreationTimeUtc(FilePath, created);
            }
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes("replacement"));

            await _provider.SaveAsync(Args(stream, true));

            Assert.Equal("replacement", await File.ReadAllTextAsync(FilePath));
            if (OperatingSystem.IsWindows())
            {
                Assert.Equal(created, File.GetCreationTimeUtc(FilePath));
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task Copy_Should_Be_Invisible_At_Final_Path_Until_Published(bool overwrite)
        {
            if (overwrite)
            {
                await File.WriteAllTextAsync(FilePath, "original");
            }
            var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            using var stream = new ControlledCopyStream(async () =>
            {
                entered.SetResult();
                await release.Task;
            });
            var save = _provider.SaveAsync(Args(stream, overwrite));
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(10));
            try
            {
                if (overwrite)
                {
                    Assert.Equal("original", await File.ReadAllTextAsync(FilePath));
                }
                else
                {
                    Assert.False(File.Exists(FilePath));
                    Assert.Empty(await _provider.GetAccessUrlAsync(new FileProviderAccessArgs("docs", _configuration, "file.txt", checkFileExist: true)));
                }
            }
            finally
            {
                release.SetResult();
                await save;
            }
            Assert.Equal("complete", await File.ReadAllTextAsync(FilePath));
            Assert.Equal(new[] { FilePath }, Directory.GetFiles(_basePath, "*", SearchOption.AllDirectories));
        }

        [Fact]
        public async Task Concurrent_Creates_Should_Publish_One_Complete_Winner()
        {
            using var entered = new SemaphoreSlim(0);
            var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            async Task Pause()
            {
                entered.Release();
                await release.Task;
            }
            using var first = new ControlledCopyStream(Pause, "first-content");
            using var second = new ControlledCopyStream(Pause, "second-content");
            var firstSave = Record.ExceptionAsync(() => _provider.SaveAsync(Args(first)));
            var secondSave = Record.ExceptionAsync(() => _provider.SaveAsync(Args(second)));
            try
            {
                Assert.True(await entered.WaitAsync(TimeSpan.FromSeconds(10)));
                Assert.True(await entered.WaitAsync(TimeSpan.FromSeconds(10)));
            }
            finally
            {
                release.SetResult();
                await Task.WhenAll(firstSave, secondSave);
            }
            var results = await Task.WhenAll(firstSave, secondSave);
            Assert.Single(results, result => result == null);
            Assert.Single(results, result => result is FileAlreadyExistsException);
            Assert.Contains(await File.ReadAllTextAsync(FilePath), new[] { "first-content", "second-content" });
            Assert.Equal(new[] { FilePath }, Directory.GetFiles(_basePath, "*", SearchOption.AllDirectories));
        }

        public void Dispose() => Directory.Delete(_basePath, true);

        private sealed class ControlledCopyStream : MemoryStream
        {
            private readonly Func<Task> _afterPrefix;
            private readonly byte[] _content;
            private readonly bool _canSeek;
            public int CopyCount { get; private set; }
            public override bool CanSeek => _canSeek;

            public ControlledCopyStream(Func<Task> afterPrefix, string content = "complete", bool canSeek = false)
            {
                _afterPrefix = afterPrefix;
                _content = Encoding.UTF8.GetBytes(content);
                _canSeek = canSeek;
            }

            public override async Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
            {
                CopyCount++;
                await destination.WriteAsync(_content.AsMemory(0, 1), cancellationToken);
                await destination.FlushAsync(cancellationToken);
                await _afterPrefix();
                await destination.WriteAsync(_content.AsMemory(1), cancellationToken);
            }
        }
    }
}
