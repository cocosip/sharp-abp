using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class FileStoringContainerMapper : IObjectMapper<FileStoringContainer, FileStoringContainerEto>, ITransientDependency
    {
        public FileStoringContainerEto Map(FileStoringContainer source)
        {
            return new FileStoringContainerEto()
            {
                Id = source.Id,
                TenantId = source.TenantId,
                Provider = source.Provider,
                Name = source.Name,
            };
        }

        public FileStoringContainerEto Map(FileStoringContainer source, FileStoringContainerEto destination)
        {
            destination.Id = source.Id;
            destination.TenantId = source.TenantId;
            destination.Provider = source.Provider;
            destination.Name = source.Name;
            return destination;
        }
    }
}
