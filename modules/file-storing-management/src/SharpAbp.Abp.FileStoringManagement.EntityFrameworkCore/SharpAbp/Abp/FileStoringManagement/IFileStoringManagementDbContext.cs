using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.FileStoringManagement
{
    [ConnectionStringName(FileStoringManagementDbProperties.ConnectionStringName)]
    public interface IFileStoringManagementDbContext : IEfCoreDbContext
    {
        DbSet<FileStoringContainer> FileStoringContainers { get; set; }

        DbSet<FileStoringContainerItem> FileStoringContainerItems { get; set; }
    }
}
