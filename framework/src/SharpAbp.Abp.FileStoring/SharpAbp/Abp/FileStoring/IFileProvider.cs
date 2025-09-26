using System.IO;
using System.Threading.Tasks;

namespace SharpAbp.Abp.FileStoring
{
    /// <summary>
    /// Defines the contract for file storage providers.
    /// This interface provides a unified way to interact with different file storage systems
    /// such as local file system, Azure Blob Storage, AWS S3, etc.
    /// </summary>
    public interface IFileProvider
    {
        /// <summary>
        /// Gets the unique identifier for this file provider.
        /// </summary>
        string Provider { get; }

        /// <summary>
        /// Saves a file to the storage provider.
        /// </summary>
        /// <param name="args">The arguments containing file information and configuration.</param>
        /// <returns>The unique identifier of the saved file.</returns>
        Task<string> SaveAsync(FileProviderSaveArgs args);

        /// <summary>
        /// Deletes a file from the storage provider.
        /// </summary>
        /// <param name="args">The arguments containing file identification and configuration.</param>
        /// <returns>True if the file was successfully deleted, false otherwise.</returns>
        Task<bool> DeleteAsync(FileProviderDeleteArgs args);

        /// <summary>
        /// Checks if a file exists in the storage provider.
        /// </summary>
        /// <param name="args">The arguments containing file identification and configuration.</param>
        /// <returns>True if the file exists, false otherwise.</returns>
        Task<bool> ExistsAsync(FileProviderExistsArgs args);

        /// <summary>
        /// Downloads a file from the storage provider to a local path.
        /// </summary>
        /// <param name="args">The arguments containing file identification, configuration, and download path.</param>
        /// <returns>True if the file was successfully downloaded, false otherwise.</returns>
        Task<bool> DownloadAsync(FileProviderDownloadArgs args);

        /// <summary>
        /// Retrieves a file stream from the storage provider.
        /// </summary>
        /// <param name="args">The arguments containing file identification and configuration.</param>
        /// <returns>A stream of the file content, or null if the file doesn't exist.</returns>
        Task<Stream?> GetOrNullAsync(FileProviderGetArgs args);

        /// <summary>
        /// Gets a direct access URL for the file.
        /// </summary>
        /// <param name="args">The arguments containing file identification, configuration, and access parameters.</param>
        /// <returns>A direct access URL for the file.</returns>
        Task<string> GetAccessUrlAsync(FileProviderAccessArgs args);
    }
}