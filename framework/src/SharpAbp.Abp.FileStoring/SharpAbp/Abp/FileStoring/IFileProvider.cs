using System.IO;
using System.Threading.Tasks;

namespace SharpAbp.Abp.FileStoring
{
    public interface IFileProvider
    {
        Task SaveAsync(FileProviderSaveArgs args);
        
        Task<bool> DeleteAsync(FileProviderDeleteArgs args);
        
        Task<bool> ExistsAsync(FileProviderExistsArgs args);
        
        Task<Stream> GetOrNullAsync(FileProviderGetArgs args);
    }
}