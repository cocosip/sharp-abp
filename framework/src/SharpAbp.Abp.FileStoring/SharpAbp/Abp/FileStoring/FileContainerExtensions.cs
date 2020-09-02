using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Volo.Abp.FileStoring
{
    public static class FileContainerExtensions
    {
        public static async Task SaveAsync(
            this IFileContainer container,
            string name,
            byte[] bytes,
            bool overrideExisting = false,
            CancellationToken cancellationToken = default
        )
        {
            using (var memoryStream = new MemoryStream(bytes))
            {
                await container.SaveAsync(
                    name,
                    memoryStream,
                    overrideExisting,
                    cancellationToken
                );
            }
        }
        
        public static async Task<byte[]> GetAllBytesAsync(
            this IFileContainer container,
            string name,
            CancellationToken cancellationToken = default)
        {
            using (var stream = await container.GetAsync(name, cancellationToken))
            {
                return await stream.GetAllBytesAsync(cancellationToken);
            }
        }
        
        public static async Task<byte[]> GetAllBytesOrNullAsync(
            this IFileContainer container,
            string name,
            CancellationToken cancellationToken = default)
        {
            var stream = await container.GetOrNullAsync(name, cancellationToken);
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