using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.FileStoring.EntityFrameworkCore
{
    [ConnectionStringName(AbpFileStoringDbProperties.ConnectionStringName)]
    public interface IFileStoringDbContext : IEfCoreDbContext
    {
        DbSet<FastDFSCluster> FastDFSClusters { get; set; }
        DbSet<FastDFSTracker> FastDFSTrackers { get; set; }
        DbSet<FastDFSGroup> FastDFSGroups { get; set; }
        DbSet<S3Node> S3Nodes { get; set; }
        DbSet<S3Bucket> S3Buckets { get; set; }
        DbSet<TenantFastDFS> TenantFastDFS { get; set; }
        DbSet<TenantS3> TenantS3 { get; set; }
    }
}
