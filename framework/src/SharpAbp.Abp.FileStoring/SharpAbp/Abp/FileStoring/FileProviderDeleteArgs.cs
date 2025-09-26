﻿using JetBrains.Annotations;
using System.Threading;

namespace SharpAbp.Abp.FileStoring
{
    /// <summary>
    /// Represents the arguments for deleting a file.
    /// </summary>
    public class FileProviderDeleteArgs : FileProviderArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileProviderDeleteArgs"/> class.
        /// </summary>
        /// <param name="containerName">The name of the container where the file is stored.</param>
        /// <param name="configuration">The configuration settings for the file container.</param>
        /// <param name="fileId">The unique identifier of the file to delete.</param>
        /// <param name="cancellationToken">The cancellation token for the operation.</param>
        public FileProviderDeleteArgs(
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