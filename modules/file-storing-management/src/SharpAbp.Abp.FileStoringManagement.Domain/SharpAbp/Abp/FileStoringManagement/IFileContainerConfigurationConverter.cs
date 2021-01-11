using JetBrains.Annotations;
using SharpAbp.Abp.FileStoring;

namespace SharpAbp.Abp.FileStoringManagement
{
    public interface IFileContainerConfigurationConverter
    {

        /// <summary>
        /// Convert database entity 'FileStoringContainer' to 'FileContainerConfiguration'
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        FileContainerConfiguration ToConfiguration(FileStoringContainer container);

        /// <summary>
        /// Convert 'FileStoringContainerCacheItem' to 'FileContainerConfiguration'
        /// </summary>
        /// <param name="cacheItem"></param>
        /// <returns></returns>
        FileContainerConfiguration ToConfiguration(FileStoringContainerCacheItem cacheItem);
    }
}
