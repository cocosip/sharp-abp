using System;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class FileStoringContainerEtoMapper : IObjectMapper<FileStoringContainer, FileStoringContainerEto>, ITransientDependency
    {
        public FileStoringContainerEto Map(FileStoringContainer source)
        {
            if (source == null)
            {
                return null;
            }
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
            if (destination == null)
            {
                destination = new FileStoringContainerEto();
            }

            if (source != null)
            {
                destination.Id = source.Id;
                destination.TenantId = source.TenantId;
                if (!source.Provider.IsNullOrWhiteSpace())
                {
                    destination.Provider = source.Provider;
                }

                if (!source.Name.IsNullOrWhiteSpace())
                {
                    destination.Name = source.Name;
                }
            }
            return destination;
        }
    }
}
