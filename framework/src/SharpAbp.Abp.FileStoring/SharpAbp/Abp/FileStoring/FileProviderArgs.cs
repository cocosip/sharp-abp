using System.Threading;
using JetBrains.Annotations;

namespace Volo.Abp.FileStoring
{
    public abstract class FileProviderArgs
    {
        [NotNull]
        public string ContainerName { get; }
        
        [NotNull]
        public FileContainerConfiguration Configuration { get; }

        [NotNull]
        public string BlobName { get; }
        
        public CancellationToken CancellationToken { get; }

        protected FileProviderArgs(
            [NotNull] string containerName,
            [NotNull] FileContainerConfiguration configuration,
            [NotNull] string blobName,
            CancellationToken cancellationToken = default)
        {
            ContainerName = Check.NotNullOrWhiteSpace(containerName, nameof(containerName));
            Configuration = Check.NotNull(configuration, nameof(configuration));
            BlobName = Check.NotNullOrWhiteSpace(blobName, nameof(blobName));
            CancellationToken = cancellationToken;
        }
    }
}
