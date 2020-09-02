using JetBrains.Annotations;

namespace Volo.Abp.FileStoring
{
    public interface IFileProviderSelector
    {
        [NotNull]
        IFileProvider Get([NotNull] string containerName);
    }
}