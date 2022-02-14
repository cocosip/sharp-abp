using JetBrains.Annotations;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;

namespace SharpAbp.Abp.FileStoring
{
    public abstract class FileProviderBase : IFileProvider
    {
        public abstract string Provider { get; }

        public abstract Task<string> SaveAsync(FileProviderSaveArgs args);

        public abstract Task<bool> DeleteAsync(FileProviderDeleteArgs args);

        public abstract Task<bool> ExistsAsync(FileProviderExistsArgs args);

        public abstract Task<bool> DownloadAsync(FileProviderDownloadArgs args);

        public abstract Task<Stream> GetOrNullAsync(FileProviderGetArgs args);

        public abstract Task<string> GetAccessUrlAsync(FileProviderAccessArgs args);

        protected virtual async Task<Stream> TryCopyToMemoryStreamAsync(
            Stream stream, 
            CancellationToken cancellationToken = default)
        {
            if (stream == null)
            {
                return null;
            }

            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream, cancellationToken);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }

        protected virtual async Task TryWriteToFileAsync(
            Stream stream, 
            [NotNull] string path, 
            CancellationToken cancellationToken = default)
        {
            Check.NotNullOrWhiteSpace(path, nameof(path));
            using var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            await stream.CopyToAsync(fs);
        }

    }
}