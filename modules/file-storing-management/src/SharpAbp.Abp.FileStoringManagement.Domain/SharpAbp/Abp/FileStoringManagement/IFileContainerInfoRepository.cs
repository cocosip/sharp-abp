using System;
using Volo.Abp.Domain.Repositories;

namespace SharpAbp.Abp.FileStoringManagement
{
    public interface IFileContainerInfoRepository : IBasicRepository<FileContainerInfo, Guid>
    {
        
    }
}
