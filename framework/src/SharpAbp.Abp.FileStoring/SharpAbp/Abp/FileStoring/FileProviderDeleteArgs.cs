using JetBrains.Annotations;
using System.Threading;

namespace SharpAbp.Abp.FileStoring
{
    public class FileProviderDeleteArgs : FileProviderArgs
    {
        public FileProviderDeleteArgs(
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