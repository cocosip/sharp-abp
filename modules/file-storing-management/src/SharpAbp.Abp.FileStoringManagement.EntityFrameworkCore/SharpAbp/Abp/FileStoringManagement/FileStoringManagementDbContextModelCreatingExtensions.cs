using Microsoft.EntityFrameworkCore;
using System;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace SharpAbp.Abp.FileStoringManagement
{
    public static class FileStoringManagementDbContextModelCreatingExtensions
    {
        public static void ConfigureFileStoringManagement(
          this ModelBuilder builder,
          Action<FileStoringManagementModelBuilderConfigurationOptions> optionsAction = null)
        {
            Check.NotNull(builder, nameof(builder));

            var options = new FileStoringManagementModelBuilderConfigurationOptions(
                FileStoringManagementDbProperties.DbTablePrefix,
                FileStoringManagementDbProperties.DbSchema
            );

            optionsAction?.Invoke(options);

            builder.Entity<FileStoringContainer>(b =>
            {
                b.ToTable(options.TablePrefix + "FileStoringContainers", options.Schema);

                b.ConfigureByConvention();

                b.Property(p => p.Title).IsRequired().HasMaxLength(FileStoringContainerConsts.MaxTitleLength);

                b.Property(p => p.Name).IsRequired().HasMaxLength(FileStoringContainerConsts.MaxNameLength);

                b.Property(p => p.ProviderName).IsRequired().HasMaxLength(FileStoringContainerConsts.MaxProviderNameLength);

                b.Property(p => p.HttpSupport).IsRequired();

                b.Property(p => p.State).IsRequired();

                b.Property(p => p.Describe).HasMaxLength(FileStoringContainerConsts.MaxDescribeLength);

                b.HasMany<FileStoringContainerItem>().WithOne().HasForeignKey(p => p.ContainerId);

                b.HasIndex(x => new { x.TenantId, x.Name });
            });

            builder.Entity<FileStoringContainerItem>(b =>
            {
                b.ToTable(options.TablePrefix + "FileStoringContainerItems", options.Schema);

                b.ConfigureByConvention();

                b.Property(p => p.ContainerId).IsRequired(); //TODO: Foreign key!
                b.Property(p => p.Name).IsRequired().HasMaxLength(FileStoringContainerItemConsts.MaxNameLength);
                b.Property(p => p.Value).IsRequired().HasMaxLength(FileStoringContainerItemConsts.MaxValueLength);
                b.Property(p => p.TypeName).IsRequired().HasMaxLength(FileStoringContainerItemConsts.MaxTypeNameLength);

                b.HasOne<FileStoringContainer>().WithMany().HasForeignKey(p => p.ContainerId);

                b.HasIndex(x => new { x.TenantId, x.ContainerId, x.Name });
            });
        }
    }
}
