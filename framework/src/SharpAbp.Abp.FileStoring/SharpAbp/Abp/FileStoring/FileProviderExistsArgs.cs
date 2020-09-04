using JetBrains.Annotations;
using System.Threading;

namespace SharpAbp.Abp.FileStoring
{
    public class FileProviderExistsArgs : FileProviderArgs
    {
        public FileProviderExistsArgs(
            [NotNull] string containerName,
            [NotNull] FileContainerConfiguration configuration,
            [NotNull] string fileId,
            CancellationToken cancellationToken = default)
        : base(
            containerName,
            configuration,
            fileId,
            cancellationToken)
        {
        }
    }
}