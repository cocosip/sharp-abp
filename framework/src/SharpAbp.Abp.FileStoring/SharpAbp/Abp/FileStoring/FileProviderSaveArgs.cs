using JetBrains.Annotations;
using System.IO;
using System.Threading;

namespace Volo.Abp.FileStoring
{
    public class FileProviderSaveArgs : FileProviderArgs
    {
        [NotNull]
        public Stream BlobStream { get; }
        
        public bool OverrideExisting { get; }

        public FileProviderSaveArgs(
            [NotNull] string containerName,
            [NotNull] FileContainerConfiguration configuration,
            [NotNull] string blobName,
            [NotNull] Stream blobStream,
            bool overrideExisting = false,
            CancellationToken cancellationToken = default)
            : base(
                containerName,
                configuration,
                blobName,
                cancellationToken)
        {
            BlobStream = Check.NotNull(blobStream, nameof(blobStream));
            OverrideExisting = overrideExisting;
        }
    }
}