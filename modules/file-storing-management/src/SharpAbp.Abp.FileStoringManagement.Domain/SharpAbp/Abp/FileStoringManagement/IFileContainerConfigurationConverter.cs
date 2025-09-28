using JetBrains.Annotations;
using SharpAbp.Abp.FileStoring;

namespace SharpAbp.Abp.FileStoringManagement
{
    /// <summary>
    /// Defines the contract for converting file storing container entities and cache items
    /// to file container configurations. This service transforms persistent data models
    /// into runtime configuration objects used by the file storage system.
    /// </summary>
    public interface IFileContainerConfigurationConverter
    {
        /// <summary>
        /// Converts a database entity 'FileStoringContainer' to a 'FileContainerConfiguration'.
        /// This method transforms the persistent container entity into a runtime configuration
        /// object that can be used by the file storage system.
        /// </summary>
        /// <param name="container">The file storing container entity to convert.</param>
        /// <returns>A file container configuration object ready for use by the file storage system.</returns>
        FileContainerConfiguration ToConfiguration(FileStoringContainer container);

        /// <summary>
        /// Converts a 'FileStoringContainerCacheItem' to a 'FileContainerConfiguration'.
        /// This method transforms cached container data into a runtime configuration object
        /// that can be used by the file storage system. This overload is optimized for
        /// cache scenarios to reduce database queries.
        /// </summary>
        /// <param name="cacheItem">The cached file storing container item to convert.</param>
        /// <returns>A file container configuration object ready for use by the file storage system.</returns>
        FileContainerConfiguration ToConfiguration(FileStoringContainerCacheItem cacheItem);
    }
}
