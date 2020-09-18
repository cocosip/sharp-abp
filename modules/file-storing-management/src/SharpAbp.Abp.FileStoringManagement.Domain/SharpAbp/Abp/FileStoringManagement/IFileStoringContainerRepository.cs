using System;
using Volo.Abp.Domain.Repositories;

namespace SharpAbp.Abp.FileStoringManagement
{
    public interface IFileStoringContainerRepository : IBasicRepository<FileStoringContainer, Guid>
    {
        
    }
}
