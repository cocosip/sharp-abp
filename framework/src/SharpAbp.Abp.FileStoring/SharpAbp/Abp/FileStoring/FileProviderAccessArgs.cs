﻿using JetBrains.Annotations;
using System;
using System.Threading;

namespace SharpAbp.Abp.FileStoring
{
    /// <summary>
    /// Represents the arguments for accessing a file, including expiration and existence check options.
    /// </summary>
    public class FileProviderAccessArgs : FileProviderArgs
    {
        /// <summary>
        /// Gets or sets the expiration time for the file access.
        /// </summary>
        public DateTime? Expires { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to check if the file exists before accessing it.
        /// </summary>
        public bool CheckFileExist { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileProviderAccessArgs"/> class.
        /// </summary>
        /// <param name="containerName">The name of the container where the file is stored.</param>
        /// <param name="configuration">The configuration settings for the file container.</param>
        /// <param name="fileId">The unique identifier of the file.</param>
        /// <param name="expires">The expiration time for the file access.</param>
        /// <param name="checkFileExist">A value indicating whether to check if the file exists before accessing it.</param>
        /// <param name="cancellationToken">The cancellation token for the operation.</param>
        public FileProviderAccessArgs(
            [NotNull] string containerName,
            [NotNull] FileContainerConfiguration configuration,
            [NotNull] string fileId,
            [CanBeNull] DateTime? expires = null,
            bool checkFileExist = false,
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