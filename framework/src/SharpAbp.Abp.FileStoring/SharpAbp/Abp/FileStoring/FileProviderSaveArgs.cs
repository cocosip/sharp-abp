using JetBrains.Annotations;
using System.IO;
using System.Threading;
using Volo.Abp;

namespace SharpAbp.Abp.FileStoring
{
    public class FileProviderSaveArgs : FileProviderArgs
    {
        [NotNull]
        public Stream FileStream { get; }
        
        public bool OverrideExisting { get; }

        public FileProviderSaveArgs(
            [NotNull] string containerName,
            [NotNull] FileContainerConfiguration configuration,
            [NotNull] string fileName,
            [NotNull] Stream fileStream,
            bool overrideExisting = false,
            CancellationToken cancellationToken = default)
            : base(
                containerName,
                configuration,
                fileName,
                cancellationToken)
        {
            FileStream = Check.NotNull(fileStream, nameof(fileStream));
            OverrideExisting = overrideExisting;
        }
    }
}