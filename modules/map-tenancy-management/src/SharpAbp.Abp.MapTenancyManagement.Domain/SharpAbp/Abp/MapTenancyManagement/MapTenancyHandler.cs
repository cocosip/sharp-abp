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
        private readonly IAllMapTenantCacheManager _allMapTenantCacheManager;
        private readonly IMapTenantCacheManager _mapTenantCacheManager;
        public MapTenancyHandler(
            IAllMapTenantCacheManager allMapTenantCacheManager,
            IMapTenantCacheManager mapTenantCacheManager)
        {
            _allMapTenantCacheManager = allMapTenantCacheManager;
            _mapTenantCacheManager = mapTenantCacheManager;
        }

        public async Task HandleEventAsync(EntityCreatedEto<MapTenantEto> eventData)
        {
            await _allMapTenantCacheManager.UpdateAsync();
            await _mapTenantCacheManager.UpdateCacheAsync(eventData.Entity.Id);
        }

        public async Task HandleEventAsync(EntityUpdatedEto<MapTenantEto> eventData)
        {
            await _allMapTenantCacheManager.UpdateAsync();
            await _mapTenantCacheManager.UpdateCacheAsync(eventData.Entity.Id);
        }

        public async Task HandleEventAsync(EntityDeletedEto<MapTenantEto> eventData)
        {
            //await _allMapTenantCacheManager.UpdateAsync();
            await _mapTenantCacheManager.RemoveCacheAsync(eventData.Entity.Code, eventData.Entity.MapCode);
        }
    }
}