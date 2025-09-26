﻿using JetBrains.Annotations;
using System.Threading;
using Volo.Abp;

namespace SharpAbp.Abp.FileStoring
{
    /// <summary>
    /// Base class for file provider arguments.
    /// Contains common properties and parameters needed for file operations.
    /// </summary>
    public abstract class FileProviderArgs
    {
        /// <summary>
        /// Gets the name of the container where the file is stored.
        /// </summary>
        [NotNull]
        public string ContainerName { get; }

        /// <summary>
        /// Gets the configuration settings for the file container.
        /// </summary>
        [NotNull]
        public FileContainerConfiguration Configuration { get; }

        /// <summary>
        /// Gets the unique identifier of the file.
        /// </summary>
        [CanBeNull]
        public string FileId { get; }

        /// <summary>
        /// Gets the cancellation token for the operation.
        /// </summary>
        public CancellationToken CancellationToken { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileProviderArgs"/> class.
        /// </summary>
        /// <param name="containerName">The name of the container where the file is stored.</param>
        /// <param name="configuration">The configuration settings for the file container.</param>
        /// <param name="fileId">The unique identifier of the file.</param>
        /// <param name="cancellationToken">The cancellation token for the operation.</param>
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