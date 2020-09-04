using JetBrains.Annotations;
using System.Threading;

namespace SharpAbp.Abp.FileStoring
{
    public class FileProviderGetArgs : FileProviderArgs
    {
        public FileProviderGetArgs(
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