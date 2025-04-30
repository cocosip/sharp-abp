using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class FileStoringHandler :
        IDistributedEventHandler<FileStoringContainerUpdatedEto>,
        IDistributedEventHandler<FileStoringContainerDeletedEto>,
        ITransientDependency
    {
        protected ICurrentTenant CurrentTenant { get; }
        protected IFileStoringContainerCacheManager FileStoringContainerCacheManager { get; }
        public FileStoringHandler(
            ICurrentTenant currentTenant,
            IFileStoringContainerCacheManager fileStoringContainerCacheManager)
        {
            CurrentTenant = currentTenant;
            FileStoringContainerCacheManager = fileStoringContainerCacheManager;
        }


        public async Task HandleEventAsync(FileStoringContainerUpdatedEto eventData)
        {
            using (CurrentTenant.Change(eventData.TenantId))
            {
                await FileStoringContainerCacheManager.RemoveAsync(eventData.OldName);
            }
        }

        public async Task HandleEventAsync(FileStoringContainerDeletedEto eventData)
        {
            using (CurrentTenant.Change(eventData.TenantId))
            {
                await FileStoringContainerCacheManager.RemoveAsync(eventData.Name);
            }
        }
    }
}
