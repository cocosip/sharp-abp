using JetBrains.Annotations;
using System;
using System.Threading;

namespace SharpAbp.Abp.FileStoring
{
    public class FileProviderAccessArgs : FileProviderArgs
    {
        public DateTime? Expires { get; set; }
        public bool CheckFileExist { get; set; }
        public FileProviderAccessArgs(
        [NotNull] string containerName,
        [NotNull] FileContainerConfiguration configuration,
        [NotNull] string fileId,
        [CanBeNull] DateTime? expires = null,
        [NotNull] bool checkFileExist = false,
        CancellationToken cancellationToken = default)
        : base(
            containerName,
            configuration,
            fileId,
            cancellationToken)
        {
            Expires = expires;
            CheckFileExist = checkFileExist;
        }
    }
}
