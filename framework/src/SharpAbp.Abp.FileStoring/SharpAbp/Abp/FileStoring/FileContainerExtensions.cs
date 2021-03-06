using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.FileStoring
{
    public static class FileContainerExtensions
    {
        public static async Task<string> SaveAsync(
            this IFileContainer container,
            string fileId,
            byte[] bytes,
            string ext,
            bool overrideExisting = false,
            CancellationToken cancellationToken = default)
        {
            using var memoryStream = new MemoryStream(bytes);
            return await container.SaveAsync(
                 fileId,
                 memoryStream,
                 ext,
                 overrideExisting,
                 cancellationToken
             );
        }

        public static async Task<string> SaveAsync(
            this IFileContainer container,
            string fileId,
            string path,
            bool overrideExisting = false,
            CancellationToken cancellationToken = default)
        {

            using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            return await container.SaveAsync(
                 fileId,
                 fileStream,
                 Path.GetExtension(path),
                 overrideExisting,
                 cancellationToken
             );
        }

        public static async Task<byte[]> GetAllBytesAsync(
            this IFileContainer container,
            string fileId,
            CancellationToken cancellationToken = default)
        {
            using var stream = await container.GetAsync(fileId, cancellationToken);
            return await stream.GetAllBytesAsync(cancellationToken);
        }

        public static async Task<byte[]> GetAllBytesOrNullAsync(
            this IFileContainer container,
            string fileId,
            CancellationToken cancellationToken = default)
        {
            var stream = await container.GetOrNullAsync(fileId, cancellationToken);
            if (stream == null)
            {
                return null;
            }

            using (stream)
            {
                return await stream.GetAllBytesAsync(cancellationToken);
            }
        }
    }
}