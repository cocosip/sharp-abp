#nullable enable
using System;
using System.IO;
using System.Text;
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

            public Task WriteToFileAsync(Stream stream, string path)
            {
                return TryWriteToFileAsync(stream, path);
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

        [Fact]
        public async Task TryWriteToFileAsync_Should_Overwrite_Existing_File_Content()
        {
            var provider = new TestFileProvider();
            var path = Path.Combine(Path.GetTempPath(), $"sharpabp-fileprovider-{Guid.NewGuid():N}.txt");

            try
            {
                await File.WriteAllTextAsync(path, "existing-long-content");

                await using var stream = new MemoryStream(Encoding.UTF8.GetBytes("new"));
                await provider.WriteToFileAsync(stream, path);

                var content = await File.ReadAllTextAsync(path);
                Assert.Equal("new", content);
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
