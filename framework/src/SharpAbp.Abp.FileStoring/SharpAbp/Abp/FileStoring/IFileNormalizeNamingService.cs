using System;

namespace SharpAbp.Abp.FileStoring
{
    /// <summary>
    /// Service interface for normalizing file naming conventions.
    /// Provides methods to standardize container names and file names according to configured naming rules.
    /// </summary>
    public interface IFileNormalizeNamingService
    {
        /// <summary>
        /// Normalizes both container name and file name based on the provided configuration.
        /// </summary>
        /// <param name="configuration">The file container configuration containing naming normalizers</param>
        /// <param name="containerName">The original container name to normalize</param>
        /// <param name="fileName">The original file name to normalize</param>
        /// <returns>A FileNormalizeNaming object containing the normalized container and file names</returns>
        FileNormalizeNaming NormalizeNaming(FileContainerConfiguration configuration, string containerName, string fileName);

        /// <summary>
        /// Normalizes a container name based on the provided configuration.
        /// </summary>
        /// <param name="configuration">The file container configuration containing naming normalizers</param>
        /// <param name="containerName">The original container name to normalize</param>
        /// <returns>The normalized container name</returns>
        string NormalizeContainerName(FileContainerConfiguration configuration, string containerName);

        /// <summary>
        /// Normalizes a file name based on the provided configuration.
        /// </summary>
        /// <param name="configuration">The file container configuration containing naming normalizers</param>
        /// <param name="fileName">The original file name to normalize</param>
        /// <returns>The normalized file name</returns>
        string NormalizeFileName(FileContainerConfiguration configuration, string fileName);
    }
}