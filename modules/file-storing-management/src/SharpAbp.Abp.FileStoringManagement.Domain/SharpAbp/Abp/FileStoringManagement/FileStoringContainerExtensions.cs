using JetBrains.Annotations;
using Volo.Abp;
using System;

namespace SharpAbp.Abp.FileStoringManagement
{
    /// <summary>
    /// Provides extension methods for <see cref="FileStoringContainer"/> entities.
    /// This static class contains utility methods for converting container entities
    /// to other representations, such as cache items, to improve system performance
    /// and facilitate data transformation operations.
    /// </summary>
    public static class FileStoringContainerExtensions
    {
        /// <summary>
        /// Converts a <see cref="FileStoringContainer"/> entity to a <see cref="FileStoringContainerCacheItem"/>.
        /// This method transforms the persistent container entity into a cache-friendly format
        /// that can be stored in distributed cache for improved performance. It copies all
        /// container properties and configuration items while ensuring data integrity.
        /// </summary>
        /// <param name="container">The file storing container entity to convert. Cannot be null.</param>
        /// <returns>
        /// A <see cref="FileStoringContainerCacheItem"/> representing the container data,
        /// or null if the input container is null or default.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when the container parameter is null.</exception>
        public static FileStoringContainerCacheItem AsCacheItem([NotNull] this FileStoringContainer container)
        {
            Check.NotNull(container, nameof(container));
            if (container == null || container == default)
            {
                return null;
            }

            var cacheItem = new FileStoringContainerCacheItem(
                container.Id,
                container.TenantId,
                container.IsMultiTenant,
                container.Provider,
                container.Name,
                container.Title,
                container.EnableAutoMultiPartUpload,
                container.MultiPartUploadMinFileSize,
                container.MultiPartUploadShardingSize,
                container.HttpAccess);

            foreach (var item in container.Items)
            {
                cacheItem.Items.Add(new FileStoringContainerItemCacheItem(
                    item.Id,
                    item.Name,
                    item.Value,
                    item.ContainerId));
            }

            return cacheItem;
        }
    }
}
