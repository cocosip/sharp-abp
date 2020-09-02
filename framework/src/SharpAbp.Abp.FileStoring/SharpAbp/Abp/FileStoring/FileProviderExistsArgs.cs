using System.Threading;
using JetBrains.Annotations;

namespace Volo.Abp.FileStoring
{
    public class FileProviderExistsArgs : FileProviderArgs
    {
        public FileProviderExistsArgs(
            [NotNull] string containerName,
            [NotNull] FileContainerConfiguration configuration,
            [NotNull] string blobName,
            CancellationToken cancellationToken = default)
        : base(
            containerName,
            configuration,
            blobName,
            cancellationToken)
        {
        }
    }
}