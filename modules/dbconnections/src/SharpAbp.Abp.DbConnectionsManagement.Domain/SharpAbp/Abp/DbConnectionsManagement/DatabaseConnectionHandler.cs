using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    public class DatabaseConnectionHandler :
        IDistributedEventHandler<DatabaseConnectionUpdatedEto>,
        IDistributedEventHandler<DatabaseConnectionNameChangedEto>,
        IDistributedEventHandler<DatabaseConnectionDeletedEto>,
        ITransientDependency
    {

        protected IDatabaseConnectionCacheManager CacheManager { get; }
        public DatabaseConnectionHandler(IDatabaseConnectionCacheManager cacheManager)
        {
            CacheManager = cacheManager;
        }

        public async Task HandleEventAsync(DatabaseConnectionUpdatedEto eventData)
        {
            await CacheManager.RemoveAsync(eventData.Name);
        }

        public async Task HandleEventAsync(DatabaseConnectionNameChangedEto eventData)
        {
            if (eventData.OldName != eventData.Name)
            {
                await CacheManager.RemoveAsync(eventData.OldName);
            }
        }

        public async Task HandleEventAsync(DatabaseConnectionDeletedEto eventData)
        {
            await CacheManager.RemoveAsync(eventData.Name);
        }
    }
}
