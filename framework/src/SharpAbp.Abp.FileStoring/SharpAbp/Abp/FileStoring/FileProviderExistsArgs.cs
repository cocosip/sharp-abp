using System.Threading;
using JetBrains.Annotations;

namespace SharpAbp.Abp.FileStoring
{
    public class FileProviderExistsArgs : FileProviderArgs
    {
        public FileProviderExistsArgs(
            [NotNull] string containerName,
            [NotNull] FileContainerConfiguration configuration,
            [NotNull] string fileName,
            CancellationToken cancellationToken = default)
        : base(
            containerName,
            configuration,
            fileName,
            cancellationToken)
        {
        }
    }
}