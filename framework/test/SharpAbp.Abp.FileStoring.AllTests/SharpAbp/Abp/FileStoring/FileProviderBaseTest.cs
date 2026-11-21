#nullable enable
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.FileStoring
{
    public class FileProviderBaseTest
    {
        private sealed class TestFileProvider : FileProviderBase
        {
            public override string Provider => "Test";

            public override Task<string> SaveAsync(FileProviderSaveArgs args)
            {
                throw new NotSupportedException();
            }

            public override Task<bool> DeleteAsync(FileProviderDeleteArgs args)
            {
                throw new NotSupportedException();
            }

            public override Task<bool> ExistsAsync(FileProviderExistsArgs args)
            {
                throw new NotSupportedException();
            }

            public override Task<bool> DownloadAsync(FileProviderDownloadArgs args)
            {
                throw new NotSupportedException();
            }

            public override Task<Stream?> GetOrNullAsync(FileProviderGetArgs args)
            {
                throw new NotSupportedException();
            }

            public override Task<string> GetAccessUrlAsync(FileProviderAccessArgs args)
            {
                throw new NotSupportedException();
            }

            public Task WriteToFileAsync(Stream stream, string path, CancellationToken cancellationToken = default)
            {
                return TryWriteToFileAsync(stream, path, cancellationToken);
            }

            public Task<bool> WriteToFileAsync(string path, Func<Stream, CancellationToken, Task<bool>> writeAsync)
            {
                return TryWriteToFileAsync(path, writeAsync, CancellationToken.None);
            }

            public Task<int> ReadToBufferAsync(Stream stream, byte[] buffer)
            {
                return ReadToBufferAsync(stream, buffer, buffer.Length);
            }
        }

        private sealed class ShortReadStream : MemoryStream
        {
            public ShortReadStream(byte[] buffer)
                : base(buffer)
            {
            }

            public override Task<int> ReadAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
            {
                return base.ReadAsync(buffer, offset, Math.Min(count, 3), cancellationToken);
            }
        }

        private sealed class InterruptedCopyStream(CancellationTokenSource? cancellation) : MemoryStream
        {
            public override async Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
            {
                await destination.WriteAsync(new byte[] { 1, 2, 3 }, cancellationToken);
                cancellation?.Cancel();
                cancellationToken.ThrowIfCancellationRequested();
                throw new IOException("Source read failed");
            }
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public async Task TryWriteToFileAsync_Should_Leave_Destination_Unchanged_After_Interrupted_Copy(
            bool existing, bool cancel)
        {
            var directory = Directory.CreateTempSubdirectory("sharpabp-download-");
            var path = Path.Combine(directory.FullName, "destination.txt");
            try
            {
                if (existing)
                {
                    await File.WriteAllTextAsync(path, "existing");
                }

                using var cancellation = new CancellationTokenSource();
                using var stream = new InterruptedCopyStream(cancel ? cancellation : null);
                var provider = new TestFileProvider();
                if (existing && !OperatingSystem.IsWindows())
                {
                    await Assert.ThrowsAsync<NotSupportedException>(() => provider.WriteToFileAsync(stream, path));
                    Assert.False(cancellation.IsCancellationRequested);
                }
                else if (cancel)
                {
                    await Assert.ThrowsAnyAsync<OperationCanceledException>(
                        () => provider.WriteToFileAsync(stream, path, cancellation.Token));
                }
                else
                {
                    await Assert.ThrowsAsync<IOException>(() => provider.WriteToFileAsync(stream, path));
                }

                if (existing)
                {
                    Assert.Equal("existing", await File.ReadAllTextAsync(path));
                    Assert.Single(directory.GetFileSystemInfos());
                }
                else
                {
                    Assert.Empty(directory.GetFileSystemInfos());
                }
            }
            finally
            {
                directory.Delete(true);
            }
        }

        [Fact]
        public async Task Unix_Download_Does_Not_Replace_A_File_Created_During_Copy()
        {
            if (OperatingSystem.IsWindows())
            {
                return;
            }

            var directory = Directory.CreateTempSubdirectory("sharpabp-download-race-");
            var path = Path.Combine(directory.FullName, "destination.txt");
            try
            {
                var provider = new TestFileProvider();
                await Assert.ThrowsAsync<NotSupportedException>(() => provider.WriteToFileAsync(path, async (output, token) =>
                {
                    await output.WriteAsync(Encoding.UTF8.GetBytes("download"), token);
                    await File.WriteAllTextAsync(path, "other writer", token);
                    return true;
                }));

                Assert.Equal("other writer", await File.ReadAllTextAsync(path));
                Assert.Single(directory.GetFileSystemInfos());
            }
            finally
            {
                directory.Delete(true);
            }
        }

        [Fact]
        public async Task TryWriteToFileAsync_Should_Replace_Existing_File_Only_On_Windows()
        {
            var provider = new TestFileProvider();
            var path = Path.Combine(Path.GetTempPath(), $"sharpabp-fileprovider-{Guid.NewGuid():N}.txt");

            try
            {
                await File.WriteAllTextAsync(path, "existing-long-content");

                await using var stream = new MemoryStream(Encoding.UTF8.GetBytes("new"));
                if (OperatingSystem.IsWindows())
                {
                    await provider.WriteToFileAsync(stream, path);
                    Assert.Equal("new", await File.ReadAllTextAsync(path));
                }
                else
                {
                    await Assert.ThrowsAsync<NotSupportedException>(() => provider.WriteToFileAsync(stream, path));
                    Assert.Equal("existing-long-content", await File.ReadAllTextAsync(path));
                }
            }
            finally
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        [Fact]
        public async Task TryWriteToFileAsync_Should_Create_Directory_When_Missing()
        {
            var provider = new TestFileProvider();
            var root = Path.Combine(Path.GetTempPath(), $"sharpabp-fileprovider-{Guid.NewGuid():N}");
            var path = Path.Combine(root, "nested", "file.txt");

            try
            {
                await using var stream = new MemoryStream(Encoding.UTF8.GetBytes("content"));
                await provider.WriteToFileAsync(stream, path);

                Assert.True(File.Exists(path));
                var content = await File.ReadAllTextAsync(path);
                Assert.Equal("content", content);
            }
            finally
            {
                if (Directory.Exists(root))
                {
                    Directory.Delete(root, true);
                }
            }
        }

        [Fact]
        public async Task ReadToBufferAsync_Should_Fill_Buffer_Across_Short_Reads()
        {
            var provider = new TestFileProvider();
            var expected = Encoding.UTF8.GetBytes("multipart-content");
            await using var stream = new ShortReadStream(expected);
            var buffer = new byte[expected.Length];

            var bytesRead = await provider.ReadToBufferAsync(stream, buffer);

            Assert.Equal(expected.Length, bytesRead);
            Assert.Equal(expected, buffer);
        }
    }
}
