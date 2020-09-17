using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.FileStoringManagement
{
    [ConnectionStringName(FileStoringManagementDbProperties.ConnectionStringName)]
    public class FileStoringManagementDbContext : AbpDbContext<FileStoringManagementDbContext>, IFileStoringManagementDbContext
    {
        public DbSet<FileContainerInfo> FileContainerInfos { get; set; }
        public DbSet<FileContainerItem> FileContainerItems { get; set; }

        public FileStoringManagementDbContext(DbContextOptions<FileStoringManagementDbContext> options)
            : base(options)
        {

        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            IFileContainerInfoRepository a = default;

            //builder.ConfigureFileStoring();
        }
    }
}
