using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.FileStoring.EntityFrameworkCore
{
    [ConnectionStringName(AbpFileStoringDbProperties.ConnectionStringName)]
    public class FileStoringDbContext : AbpDbContext<FileStoringDbContext>, IFileStoringDbContext
    {
        public DbSet<FastDFSCluster> FastDFSClusters { get; set; }
        public DbSet<FastDFSTracker> FastDFSTrackers { get; set; }
        public DbSet<FastDFSGroup> FastDFSGroups { get; set; }
        public DbSet<S3Node> S3Nodes { get; set; }
        public DbSet<S3Bucket> S3Buckets { get; set; }
        public DbSet<TenantFastDFS> TenantFastDFS { get; set; }
        public DbSet<TenantS3> TenantS3 { get; set; }


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
