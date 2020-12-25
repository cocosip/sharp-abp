using JetBrains.Annotations;
using Volo.Abp;

namespace SharpAbp.Abp.FileStoringManagement
{
    public static class FileStoringContainerExtensions
    {
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
