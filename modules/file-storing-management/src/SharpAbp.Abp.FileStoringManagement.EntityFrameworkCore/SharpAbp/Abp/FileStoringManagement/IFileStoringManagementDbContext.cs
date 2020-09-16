using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.FileStoringManagement
{
    [ConnectionStringName(FileStoringManagementDbProperties.ConnectionStringName)]
    public interface IFileStoringManagementDbContext : IEfCoreDbContext
    {
        DbSet<FileContainerInfo> FileContainerInfos { get; set; }

        DbSet<FileContainerItem> FileContainerItems { get; set; }
    }
}
