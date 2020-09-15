using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.FileStoring.Database
{
    [ConnectionStringName(FileStoringDatabaseDbProperties.ConnectionStringName)]
    public interface IFileStoringDbContext : IEfCoreDbContext
    {
        DbSet<DatabaseFileContainer> FileContainers { get; }

        DbSet<DatabaseFile> Files { get; }
    }
}
