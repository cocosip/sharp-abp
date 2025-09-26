﻿using JetBrains.Annotations;
using System.Threading;

namespace SharpAbp.Abp.FileStoring
{
    /// <summary>
    /// Represents the arguments for downloading a file to a specific path.
    /// </summary>
    public class FileProviderDownloadArgs : FileProviderArgs
    {
        /// <summary>
        /// Gets the path where the file will be downloaded.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileProviderDownloadArgs"/> class.
        /// </summary>
        /// <param name="containerName">The name of the container where the file is stored.</param>
        /// <param name="configuration">The configuration settings for the file container.</param>
        /// <param name="fileId">The unique identifier of the file to download.</param>
        /// <param name="path">The path where the file will be downloaded.</param>
        /// <param name="cancellationToken">The cancellation token for the operation.</param>
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