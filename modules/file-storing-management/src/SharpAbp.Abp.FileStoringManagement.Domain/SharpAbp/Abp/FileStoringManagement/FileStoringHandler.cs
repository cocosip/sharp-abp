using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class FileStoringHandler :
        IDistributedEventHandler<EntityUpdatedEto<FileStoringContainerEto>>,
        IDistributedEventHandler<EntityDeletedEto<FileStoringContainerEto>>,
        ITransientDependency
    {
        private readonly ICurrentTenant _currentTenant;
        private readonly IFileStoringContainerCacheManager _containerCacheManager;
        public FileStoringHandler(
            ICurrentTenant currentTenant,
            IFileStoringContainerCacheManager containerCacheManager)
        {
            _currentTenant = currentTenant;
            _containerCacheManager = containerCacheManager;
        }

        public async Task HandleEventAsync(EntityUpdatedEto<FileStoringContainerEto> eventData)
        {
            eventData.IsMultiTenant(out Guid? tenantId);
            using (_currentTenant.Change(tenantId))
            {
                await _containerCacheManager.UpdateAsync(eventData.Entity.Id);
            }
        }

        public async Task HandleEventAsync(EntityDeletedEto<FileStoringContainerEto> eventData)
        {
            eventData.IsMultiTenant(out Guid? tenantId);
            using (_currentTenant.Change(tenantId))
            {
                await _containerCacheManager.RemoveAsync(eventData.Entity.Name);
            }
        }
    }
}
