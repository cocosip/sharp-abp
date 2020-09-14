using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.FileStoring.EntityFrameworkCore
{
    [ConnectionStringName(AbpFileStoringDbProperties.ConnectionStringName)]
    public class FileStoringDbContext : AbpDbContext<FileStoringDbContext>, IFileStoringDbContext
    {


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
