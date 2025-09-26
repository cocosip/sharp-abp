﻿using JetBrains.Annotations;
using System.Threading;

namespace SharpAbp.Abp.FileStoring
{
    /// <summary>
    /// Represents the arguments for checking if a file exists.
    /// </summary>
    public class FileProviderExistsArgs : FileProviderArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileProviderExistsArgs"/> class.
        /// </summary>
        /// <param name="containerName">The name of the container where the file is stored.</param>
        /// <param name="configuration">The configuration settings for the file container.</param>
        /// <param name="fileId">The unique identifier of the file.</param>
        /// <param name="cancellationToken">The cancellation token for the operation.</param>
        public FileProviderExistsArgs(
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