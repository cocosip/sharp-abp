using JetBrains.Annotations;
using System.Threading;

namespace SharpAbp.Abp.FileStoring
{
    public class FileProviderDownloadArgs : FileProviderArgs
    {
        public string Path { get; }
        public FileProviderDownloadArgs(
           [NotNull] string containerName,
           [NotNull] FileContainerConfiguration configuration,
           [NotNull] string fileId,
           [NotNull] string path,
           CancellationToken cancellationToken = default)
           : base(
               containerName,
               configuration,
               fileId,
               cancellationToken)
        {
            Path = path;
        }
    }
}
