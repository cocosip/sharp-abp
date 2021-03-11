using Microsoft.EntityFrameworkCore;
using System;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace SharpAbp.Abp.FileStoringManagement.EntityFrameworkCore
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

                b.Property(p => p.Provider).IsRequired().HasMaxLength(FileStoringContainerConsts.MaxProviderLength);

                b.Property(p => p.HttpAccess).IsRequired();

                b.Property(p => p.IsMultiTenant).IsRequired();

                b.HasMany(p => p.Items).WithOne().HasForeignKey(p => p.ContainerId).IsRequired();

                b.HasIndex(p => new { p.TenantId, p.Name }).IsUnique();

            });

            builder.Entity<FileStoringContainerItem>(b =>
            {
                b.ToTable(options.TablePrefix + "FileStoringContainerItems", options.Schema);

                b.ConfigureByConvention();

                b.Property(p => p.ContainerId).IsRequired(); 

                b.Property(p => p.Name).IsRequired().HasMaxLength(FileStoringContainerItemConsts.MaxNameLength);

                b.Property(p => p.Value).IsRequired().HasMaxLength(FileStoringContainerItemConsts.MaxValueLength);

                b.HasIndex(p => new { p.ContainerId, p.Name });
            });
        }
    }
}
