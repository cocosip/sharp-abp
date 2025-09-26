﻿using JetBrains.Annotations;
using System.IO;
using System.Threading;
using Volo.Abp;

namespace SharpAbp.Abp.FileStoring
{
    /// <summary>
    /// Represents the arguments for saving a file.
    /// </summary>
    public class FileProviderSaveArgs : FileProviderArgs
    {
        /// <summary>
        /// Gets the stream containing the file data to be saved.
        /// </summary>
        public Stream? FileStream { get; }

        /// <summary>
        /// Gets or sets the file extension.
        /// </summary>
        public string? FileExt { get; set; }

        /// <summary>
        /// Gets a value indicating whether to override the file if it already exists.
        /// </summary>
        public bool OverrideExisting { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileProviderSaveArgs"/> class.
        /// </summary>
        /// <param name="containerName">The name of the container where the file is stored.</param>
        /// <param name="configuration">The configuration settings for the file container.</param>
        /// <param name="fileId">The unique identifier of the file.</param>
        /// <param name="overrideExisting">A value indicating whether to override the file if it already exists.</param>
        /// <param name="cancellationToken">The cancellation token for the operation.</param>
        public FileProviderSaveArgs(
            [NotNull] string containerName,
            [NotNull] FileContainerConfiguration configuration,
            [CanBeNull] string? fileId,
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


        /// <summary>
        /// Initializes a new instance of the <see cref="FileProviderSaveArgs"/> class with file stream and extension.
        /// </summary>
        /// <param name="containerName">The name of the container where the file is stored.</param>
        /// <param name="configuration">The configuration settings for the file container.</param>
        /// <param name="fileId">The unique identifier of the file.</param>
        /// <param name="fileStream">The stream containing the file data to be saved.</param>
        /// <param name="fileExt">The file extension.</param>
        /// <param name="overrideExisting">A value indicating whether to override the file if it already exists.</param>
        /// <param name="cancellationToken">The cancellation token for the operation.</param>
        public FileProviderSaveArgs(
           [NotNull] string containerName,
           [NotNull] FileContainerConfiguration configuration,
           [CanBeNull] string? fileId,
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