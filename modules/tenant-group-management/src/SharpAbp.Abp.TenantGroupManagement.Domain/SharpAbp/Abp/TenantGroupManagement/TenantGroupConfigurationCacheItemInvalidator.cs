using System.Threading.Tasks;
using SharpAbp.Abp.TenancyGrouping;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Local;

namespace SharpAbp.Abp.TenantGroupManagement
{
    [LocalEventHandlerOrder(-2)]
    public class TenantGroupConfigurationCacheItemInvalidator :
        ILocalEventHandler<EntityChangedEventData<TenantGroup>>,
        ILocalEventHandler<EntityDeletedEventData<TenantGroup>>,
        ILocalEventHandler<TenantGroupChangedEvent>,
        ITransientDependency
    {
        protected ITenantGroupCacheManager TenantGroupCacheManager { get; }
        public TenantGroupConfigurationCacheItemInvalidator(ITenantGroupCacheManager tenantGroupCacheManager)
        {
            TenantGroupCacheManager = tenantGroupCacheManager;
        }

        public virtual async Task HandleEventAsync(EntityChangedEventData<TenantGroup> eventData)
        {
            await TenantGroupCacheManager.RemoveAsync(eventData.Entity.Id, eventData.Entity.NormalizedName);
        }

        public async Task HandleEventAsync(EntityDeletedEventData<TenantGroup> eventData)
        {
            await TenantGroupCacheManager.RemoveAsync(eventData.Entity.Id, eventData.Entity.NormalizedName);
        }


        public virtual async Task HandleEventAsync(TenantGroupChangedEvent eventData)
        {
            await TenantGroupCacheManager.RemoveAsync(eventData.Id, eventData.NormalizedName);
        }

   
    }
}
