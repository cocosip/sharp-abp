using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class FileStoringHandler :
        IDistributedEventHandler<EntityCreatedEto<FileStoringContainerEto>>,
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
        public async Task HandleEventAsync(EntityCreatedEto<FileStoringContainerEto> eventData)
        {
            eventData.IsMultiTenant(out Guid? tenantId);
            await UpdateCacheAsync(tenantId, eventData.Entity.Id);
        }
        public async Task HandleEventAsync(EntityUpdatedEto<FileStoringContainerEto> eventData)
        {
            eventData.IsMultiTenant(out Guid? tenantId);
            await UpdateCacheAsync(tenantId, eventData.Entity.Id);
        }
        public async Task HandleEventAsync(EntityDeletedEto<FileStoringContainerEto> eventData)
        {
            eventData.IsMultiTenant(out Guid? tenantId);
            await UpdateCacheAsync(tenantId, eventData.Entity.Id);
        }

        private async Task UpdateCacheAsync(Guid? tenantId, Guid id)
        {
            using (_currentTenant.Change(tenantId))
            {
                await _containerCacheManager.UpdateAsync(id);
            }
        }
    }
}
