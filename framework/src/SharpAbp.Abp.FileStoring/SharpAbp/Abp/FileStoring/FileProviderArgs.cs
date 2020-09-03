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

        [NotNull]
        public string FileName { get; }
        
        public CancellationToken CancellationToken { get; }

        protected FileProviderArgs(
            [NotNull] string containerName,
            [NotNull] FileContainerConfiguration configuration,
            [NotNull] string fileName,
            CancellationToken cancellationToken = default)
        {
            ContainerName = Check.NotNullOrWhiteSpace(containerName, nameof(containerName));
            Configuration = Check.NotNull(configuration, nameof(configuration));
            FileName = Check.NotNullOrWhiteSpace(fileName, nameof(fileName));
            CancellationToken = cancellationToken;
        }
    }
}
