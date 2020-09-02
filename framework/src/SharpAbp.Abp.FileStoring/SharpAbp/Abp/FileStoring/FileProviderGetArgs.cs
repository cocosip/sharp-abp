using JetBrains.Annotations;
using System.Threading;

namespace Volo.Abp.FileStoring
{
    public class FileProviderGetArgs : FileProviderArgs
    {
        public FileProviderGetArgs(
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