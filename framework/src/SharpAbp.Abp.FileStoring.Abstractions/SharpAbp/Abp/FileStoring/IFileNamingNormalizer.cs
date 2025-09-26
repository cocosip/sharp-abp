﻿using System.ComponentModel;

namespace SharpAbp.Abp.FileStoring
{
    /// <summary>
    /// Provides methods for normalizing file and container names according to specific rules.
    /// </summary>
    public interface IFileNamingNormalizer
    {
        /// <summary>
        /// Normalizes the container name according to specific rules.
        /// </summary>
        /// <param name="containerName">The container name to normalize.</param>
        /// <returns>The normalized container name.</returns>
        string NormalizeContainerName(string containerName);

        /// <summary>
        /// Normalizes the file name according to specific rules.
        /// </summary>
        /// <param name="fileName">The file name to normalize.</param>
        /// <returns>The normalized file name.</returns>
        string NormalizeFileName(string fileName);
    }
}