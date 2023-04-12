using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.EventBus.Distributed;

namespace SharpAbp.Abp.TenantGroupManagement
{
    public class TenantGroupHandler :
        IDistributedEventHandler<EntityUpdatedEto<TenantGroupEto>>,
        IDistributedEventHandler<EntityDeletedEto<TenantGroupEto>>,
        ITransientDependency
    {
        private readonly ITenantGroupCacheManager _tenantGroupCacheManager;

        public TenantGroupHandler(ITenantGroupCacheManager tenantGroupCacheManager)
        {
            _tenantGroupCacheManager = tenantGroupCacheManager;
        }

        public async Task HandleEventAsync(EntityUpdatedEto<TenantGroupEto> eventData)
        {
            await _tenantGroupCacheManager.UpdateTenantGroupCacheAsync(eventData.Entity.Id);
        }

        public async Task HandleEventAsync(EntityDeletedEto<TenantGroupEto> eventData)
        {
            await _tenantGroupCacheManager.RemoveTenantGroupCacheAsync(eventData.Entity.Id, eventData.Entity.Name, eventData.Entity.Tenants);
        }
    }
}
