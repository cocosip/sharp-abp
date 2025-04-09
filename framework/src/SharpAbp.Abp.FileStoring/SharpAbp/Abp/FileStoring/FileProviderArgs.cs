using JetBrains.Annotations;
using System.Threading;
using Volo.Abp;

namespace SharpAbp.Abp.FileStoring
{
    public abstract class FileProviderArgs
    {
        [NotNull]
        public string ContainerName { get; }

        [NotNull]
        public FileContainerConfiguration Configuration { get; }

        [CanBeNull]
        public string FileId { get; }

        public CancellationToken CancellationToken { get; }

        protected FileProviderArgs(
            [NotNull] string containerName,
            [NotNull] FileContainerConfiguration configuration,
            [CanBeNull] string? fileId,
            CancellationToken cancellationToken = default)
        {
            ContainerName = Check.NotNullOrWhiteSpace(containerName, nameof(containerName));
            Configuration = Check.NotNull(configuration, nameof(configuration));
            FileId = Check.NotNullOrWhiteSpace(fileId, nameof(fileId));
            CancellationToken = cancellationToken;
        }
    }
}
