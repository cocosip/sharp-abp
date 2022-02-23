using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.EventBus.Distributed;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class MapTenancyHandler :
        IDistributedEventHandler<EntityCreatedEto<MapTenantEto>>,
        IDistributedEventHandler<EntityUpdatedEto<MapTenantEto>>,
        IDistributedEventHandler<EntityDeletedEto<MapTenantEto>>,
        ITransientDependency
    {
        private readonly IMapTenantCacheManager _mapTenantCacheManager;
        public MapTenancyHandler(
            IMapTenantCacheManager mapTenantCacheManager)
        {
            _mapTenantCacheManager = mapTenantCacheManager;
        }

        public async Task HandleEventAsync(EntityCreatedEto<MapTenantEto> eventData)
        {
            await _mapTenantCacheManager.UpdateAllCacheAsync();
        }

        public async Task HandleEventAsync(EntityUpdatedEto<MapTenantEto> eventData)
        {
            await _mapTenantCacheManager.UpdateAsync(eventData.Entity.Id);
            await _mapTenantCacheManager.UpdateAllCacheAsync();
        }

        public async Task HandleEventAsync(EntityDeletedEto<MapTenantEto> eventData)
        {
            await _mapTenantCacheManager.RemoveAsync(eventData.Entity.Code, eventData.Entity.MapCode);
            await _mapTenantCacheManager.UpdateAllCacheAsync();
        }
    }
}