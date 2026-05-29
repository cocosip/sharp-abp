using System.Threading.Tasks;

namespace SharpAbp.Abp.FileStoring
{
    public interface IFilePathContextResolver
    {
        Task<FilePathContext?> ResolveAsync();
    }
}
