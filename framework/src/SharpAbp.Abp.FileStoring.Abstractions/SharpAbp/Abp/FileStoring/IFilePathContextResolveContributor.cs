using System.Threading.Tasks;

namespace SharpAbp.Abp.FileStoring
{
    public interface IFilePathContextResolveContributor
    {
        string Name { get; }

        Task ResolveAsync(IFilePathContextResolveContext context);
    }
}
