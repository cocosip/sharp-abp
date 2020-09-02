using System.Threading;
using JetBrains.Annotations;

namespace Volo.Abp.FileStoring
{
    public class FileProviderDeleteArgs : FileProviderArgs
    {
        public FileProviderDeleteArgs(
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