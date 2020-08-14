using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace SharpAbp.Abp.FileStoring.EntityFrameworkCore
{
    public static class AbpFileStoringDbContextModelCreatingExtensions
    {
        public static void ConfigureFileStoring(
           this ModelBuilder builder,
           [CanBeNull] Action<AbpFileStoringModelBuilderConfigurationOptions> optionsAction = null)
        {
            Check.NotNull(builder, nameof(builder));

            var options = new AbpFileStoringModelBuilderConfigurationOptions(
                AbpFileStoringDbProperties.DbTablePrefix,
                AbpFileStoringDbProperties.DbSchema
            );

            optionsAction?.Invoke(options);

            builder.Entity<FastDFSCluster>(b =>
            {
                b.ToTable(options.TablePrefix + "FastDFSClusters", options.Schema);

                b.ConfigureByConvention();

                b.HasKey(x => x.Id);

                b.Property(x => x.ClusterName).IsRequired().HasMaxLength(FastDFSClusterConsts.MaxClusterNameLength).HasComment("FastDFS集群名称");

                b.Property(x => x.HttpAccessUrl).IsRequired().HasMaxLength(FastDFSClusterConsts.MaxHttpAccessUrlLength).HasComment("FastDFS集群Nginx访问地址");

                b.HasMany(x => x.Trackers).WithOne().HasForeignKey(x => x.ClusterId).IsRequired();
                b.HasMany(x => x.Groups).WithOne().HasForeignKey(x => x.ClusterId).IsRequired();
            });

            builder.Entity<FastDFSTracker>(b =>
            {
                b.ToTable(options.TablePrefix + "FastDFSTrackers", options.Schema);

                b.ConfigureByConvention();

                b.HasKey(x => x.Id);

                b.Property(x => x.ClusterId).IsRequired().HasComment("FastDFS 集群id");

                b.Property(x => x.IPAddress).IsRequired().HasMaxLength(FastDFSTrackerConsts.MaxIPAddressLength).HasComment("FastDFS Tracker ip 地址");

                b.Property(x => x.Port).IsRequired();
            });

            builder.Entity<FastDFSGroup>(b =>
            {
                b.ToTable(options.TablePrefix + "FastDFSGroups", options.Schema);

                b.ConfigureByConvention();

                b.HasKey(x => x.Id);

                b.Property(x => x.ClusterId).IsRequired().HasComment("FastDFS 集群id");

                b.Property(x => x.GroupName).IsRequired().HasMaxLength(FastDFSGroupConsts.MaxGroupNameLength).HasComment("FastDFS group 组名");
            });

            builder.Entity<TenantFastDFS>(b =>
            {
                b.ToTable(options.TablePrefix + "TenantFastDFS", options.Schema);

                b.ConfigureByConvention();

                b.HasKey(x => x.Id);

                b.Property(x => x.Name).IsRequired().HasMaxLength(TenantFastDFSConsts.MaxNameLength).HasComment("多租户下FastDFS存储信息");

                b.Property(x => x.ClusterId).IsRequired().HasComment("多租户下FastDFS存储的集群id");

                b.Property(x => x.GroupId).IsRequired().HasComment("多租户下FastDFS存储的group id");
            });

            builder.Entity<S3Node>(b =>
            {
                b.ToTable(options.TablePrefix + "S3Nodes", options.Schema);

                b.ConfigureByConvention();

                b.HasKey(x => x.Id);

                b.Property(x => x.NodeName).IsRequired().HasMaxLength(S3NodeConsts.MaxNodeNameLength).HasComment("S3存储节点名称");

                b.Property(x => x.ServerUrl).IsRequired().HasMaxLength(S3NodeConsts.MaxServerUrlLength).HasComment("S3存储节点访问地址");

                b.HasMany(x => x.Buckets).WithOne().HasForeignKey(x => x.S3NodeId).IsRequired();
            });

            builder.Entity<S3Bucket>(b =>
            {
                b.ToTable(options.TablePrefix + "S3Buckets", options.Schema);

                b.ConfigureByConvention();

                b.HasKey(x => x.Id);

                b.Property(x => x.S3NodeId).IsRequired().HasComment("S3 bucket 对应S3节点id");

                b.Property(x => x.BucketName).IsRequired().HasMaxLength(S3BucketConsts.MaxBucketNameLength).HasComment("S3 bucket名称");

                b.Property(x => x.ServerUrl).IsRequired().HasMaxLength(S3BucketConsts.MaxServerUrlLength).HasComment("bucket访问地址");

                b.Property(x => x.AccessKeyId).IsRequired().HasMaxLength(S3BucketConsts.MaxAccessKeyIdLength).HasComment("AK信息");

                b.Property(x => x.SecretAccessKey).IsRequired().HasMaxLength(S3BucketConsts.MaxSecretAccessKeyLength).HasComment("SK信息");

                b.Property(x => x.ForcePathStyle).IsRequired().HasComment("ForcePathStyle");

                b.Property(x => x.Protocol).IsRequired().HasComment("协议:1-Http,2-Https");

                b.Property(x => x.UseChunkEncoding).IsRequired().HasComment("上传块是否进行编码,当时用阿里云OSS存储时,需要设置为true");

                b.Property(x => x.VendorName).IsRequired().HasMaxLength(S3BucketConsts.MaxVendorVersionLength).HasComment("供应商名称,金山云需要使用Kingsoft360,其他的暂时随意");

                b.Property(x => x.VendorVersion).IsRequired().HasMaxLength(S3BucketConsts.MaxVendorVersionLength).HasComment("供应商版本号(根据此版本号可以设置SDK的版本)");

                b.Property(x => x.SliceSize).IsRequired().HasComment("分片的文件大小,超过该大小时,自动进行分片上传");

                b.Property(x => x.SignatureVersion).IsRequired().HasMaxLength(S3BucketConsts.MaxSignatureVersionLength).HasComment("签名版本号,大部分情况下可以使用2.0");

                b.Property(x => x.AccessUrl).IsRequired(false).HasMaxLength(S3BucketConsts.MaxAccessUrlLength).HasComment("访问的url地址,部分情况下可使用该地址拼接访问");

            });

            builder.Entity<TenantS3>(b =>
            {
                b.ToTable(options.TablePrefix + "S3Buckets", options.Schema);

                b.ConfigureByConvention();

                b.HasKey(x => x.Id);

                b.Property(x => x.NodeId).IsRequired().HasComment("多租户下S3存储节点id");

                b.Property(x => x.BucketId).IsRequired().HasComment("多租户下S3存储bucket id");
            });
        }
    }
}
