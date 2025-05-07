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
        protected IMapTenantStore MapTenantStore { get; }
        public MapTenancyHandler(IMapTenantStore mapTenantStore)
        {
            MapTenantStore = mapTenantStore;
        }

        public async Task HandleEventAsync(EntityCreatedEto<MapTenantEto> eventData)
        {
            await MapTenantStore.ResetAsync(true);
        }

        public async Task HandleEventAsync(EntityUpdatedEto<MapTenantEto> eventData)
        {
            await MapTenantStore.ResetAsync(true);
        }

        public async Task HandleEventAsync(EntityDeletedEto<MapTenantEto> eventData)
        {
            await MapTenantStore.ResetAsync(true);
        }
    }
}