using System.IO;
using System.Threading.Tasks;

namespace SharpAbp.Abp.FileStoring
{
    public abstract class FileProviderBase : IFileProvider
    {
        public abstract Task SaveAsync(FileProviderSaveArgs args);

        public abstract Task<bool> DeleteAsync(FileProviderDeleteArgs args);

        public abstract Task<bool> ExistsAsync(FileProviderExistsArgs args);

        public abstract Task<Stream> GetOrNullAsync(FileProviderGetArgs args);
    }
}