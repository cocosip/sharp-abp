using JetBrains.Annotations;

namespace SharpAbp.Abp.FileStoring
{
    public interface IFileProviderSelector
    {
        [NotNull]
        IFileProvider Get([NotNull] string containerName);
    }
}