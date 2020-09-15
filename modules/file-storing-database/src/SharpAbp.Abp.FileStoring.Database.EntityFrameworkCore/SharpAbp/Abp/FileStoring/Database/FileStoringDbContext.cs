using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.FileStoring.Database
{
    [ConnectionStringName(FileStoringDatabaseDbProperties.ConnectionStringName)]
    public class FileStoringDbContext : AbpDbContext<FileStoringDbContext>, IFileStoringDbContext
    {
        public DbSet<DatabaseFileContainer> FileContainers { get; set; }

        public DbSet<DatabaseFile> Files { get; set; }

        public FileStoringDbContext(DbContextOptions<FileStoringDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ConfigureFileStoring();
        }

    }
}
