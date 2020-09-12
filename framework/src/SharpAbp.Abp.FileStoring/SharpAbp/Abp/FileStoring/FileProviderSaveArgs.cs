using JetBrains.Annotations;
using System.IO;
using System.Threading;
using Volo.Abp;

namespace SharpAbp.Abp.FileStoring
{
    public class FileProviderSaveArgs : FileProviderArgs
    {
        public Stream FileStream { get; }

        public string FileExt { get; set; }

        public bool OverrideExisting { get; }

        public FileProviderSaveArgs(
            [NotNull] string containerName,
            [NotNull] FileContainerConfiguration configuration,
            [CanBeNull] string fileId,
            bool overrideExisting = false,
            CancellationToken cancellationToken = default)
            : base(
                containerName,
                configuration,
                fileId,
                cancellationToken)
        {
            OverrideExisting = overrideExisting;
        }


        public FileProviderSaveArgs(
           [NotNull] string containerName,
           [NotNull] FileContainerConfiguration configuration,
           [CanBeNull] string fileId,
           [NotNull] Stream fileStream,
           [NotNull] string fileExt,
           bool overrideExisting = false,
           CancellationToken cancellationToken = default)
           : this(containerName, configuration, fileId, overrideExisting, cancellationToken)
        {
            FileStream = Check.NotNull(fileStream, nameof(fileStream));
            FileExt = Check.NotNullOrWhiteSpace(fileExt, nameof(fileExt));
        }


    }
}